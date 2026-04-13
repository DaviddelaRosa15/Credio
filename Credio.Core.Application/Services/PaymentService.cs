using AutoMapper;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.Payment;
using Credio.Core.Application.Features.Payment.Commands.RegisterPayment;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAmortizationCalculatorService _amortizationService;
        private readonly IAmortizationScheduleRepository _amortizationSchedulesRepository;
        private readonly IAmortizationStatusRepository _amortizationStatusRepository;
        private readonly ILateFeeStatusRepository _lateFeeStatusRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanStatusRepository _loanStatusRepository;
        private readonly ILogger<PaymentService> _logger;
        private readonly IMapper _mapper;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly IReceiptNumberGeneratorService _receiptNumberGeneratorService;

        public PaymentService(
            IAmortizationCalculatorService amortizationService,
            IAmortizationScheduleRepository amortizationSchedulesRepository,
            IAmortizationStatusRepository amortizationStatusRepository,
            ILateFeeStatusRepository lateFeeStatusRepository,
            ILoanRepository loanRepository,
            ILoanStatusRepository loanStatusRepository,
            ILogger<PaymentService> logger,
            IMapper mapper,
            IPaymentRepository paymentRepository,
            IPaymentStatusRepository paymentStatusRepository,
            IReceiptNumberGeneratorService receiptNumberGeneratorService)
        {
            _amortizationService = amortizationService;
            _amortizationSchedulesRepository = amortizationSchedulesRepository;
            _amortizationStatusRepository = amortizationStatusRepository;
            _lateFeeStatusRepository = lateFeeStatusRepository;
            _loanRepository = loanRepository;
            _loanStatusRepository = loanStatusRepository;
            _logger = logger;
            _mapper = mapper;
            _paymentRepository = paymentRepository;
            _paymentStatusRepository = paymentStatusRepository;
            _receiptNumberGeneratorService = receiptNumberGeneratorService;
        }

        public async Task<RegisterPaymentResponseDTO> GetPaymentReceiptSnapshotAsync(string paymentId)
        {
            var payment = await _paymentRepository.GetByIdWithIncludeAsync(p => p.Id == paymentId,
                p => p.Include(p => p.PaymentMethod));

            var loan = await _loanRepository.GetByIdWithIncludeAsync(l => l.Id == payment.LoanId,
                l => l.Include(l => l.Client)
                .Include(l => l.LoanBalance)
                .Include(l => l.AmortizationSchedules).ThenInclude(s => s.AmortizationStatus));

            var response = new RegisterPaymentResponseDTO
            {
                PaymentId = payment.Id,
                ReceiptNumber = payment.ReceiptNumber,
                PaymentDate = DateOnly.FromDateTime((DateTime)payment.PaymentDate),
                ClientName = $"{loan.Client.FirstName} {loan.Client.LastName}",
                LoanNumber = loan.LoanNumber,
                PaymentMethod = payment.PaymentMethod.Name,
                AmountPaid = payment.AmountPaid,
                RemainingBalance = loan.LoanBalance.PrincipalBalance,
                PaidInstallmentsCount = loan.AmortizationSchedules.Count(s => s.AmortizationStatus.Name == "Pagada"),
                TotalInstallmentsCount = loan.AmortizationSchedules.Count()
            };

            return response;
        }

        public async Task ProcessPaymentAsync(string loanId, Payment payment)
        {
            try
            {
                var completeStatus = await _paymentStatusRepository.GetByPropertyAsync(s => s.Name == PaymentStatuses.Completado);

                // 1. Obtrener el préstamo con su cronograma de amortización
                var loan = await _loanRepository.GetByIdWithIncludeAsync(l => l.Id == loanId,
                    loan => loan.Include(l => l.LoanBalance)
                    .Include(l => l.PaymentFrequency)
                    .Include(l => l.AmortizationSchedules).ThenInclude(s => s.LateFees));

                // Asignar el monto del pago al estado de distribución
                PaymentDistributionState state = new()
                {
                    RemainingAmount = (decimal)payment.AmountPaid
                };

                // 2. Aplicar distribución en cascada (Waterfall)
                var result = await ApplyWaterfallDistribution(loan, state);

                // 3. Si hay sobrante -> Aplicar Abono a Capital y Reamortizar
                if (result.ApplyExtraToPrincipal)
                {
                    await HandleExtraPrincipal(result, loan);
                }

                // 4. Consolidar balances finales
                await SyncLoanBalances(result, loanId);

                //5. Marcar el pago como "Completado"
                payment.PaymentStatusId = completeStatus.Id;
                await _paymentRepository.UpdateAsync(payment);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Payment> RegisterInitialPaymentAsync(RegisterPaymentCommand command, string collectorId, int loanNumber)
        {
            // Obtener el estado "Pendiente" para asignarlo al nuevo pago
            var pendingStatus = await _paymentStatusRepository.GetByPropertyAsync(ps => ps.Name == PaymentStatuses.Pendiente);

            // 1. Crear el pago
            var payment = new Payment
            {
                LoanId = command.LoanId,
                EmployeeId = collectorId,
                AmountPaid = command.AmountPaid,
                PaymentMethodId = command.PaymentMethodId,
                GpsLatitude = command.GpsLatitude,
                GpsLongitude = command.GpsLongitude,
                PaymentDate = DateTime.Now,
                PaymentStatusId = pendingStatus.Id,
                ReceiptNumber = await _receiptNumberGeneratorService.GenerateReceiptNumberAsync(loanNumber.ToString(), command.LoanId)
            };

            // 2. Guardar el pago en la base de datos
            await _paymentRepository.AddAsync(payment);
            _logger.LogInformation("Pago registrado para el préstamo {LoanId} por el monto {AmountPaid}", command.LoanId, command.AmountPaid);

            return payment;
        }

        // Métodos privados para cada paso de la cascada...
        private async Task<PaymentDistributionState> ApplyWaterfallDistribution(Domain.Entities.Loan loan, PaymentDistributionState state)
        {
            try
            {
                // Obtener los estados "Pagada" para amortización y mora
                var paidStatus = await _amortizationStatusRepository.GetByPropertyAsync(s => s.Name == "Pagada");
                var paidFeeStatus = await _lateFeeStatusRepository.GetByPropertyAsync(s => s.Name == "Pagada");

                // Obtener las cuotas de amortización pendientes ordenadas por fecha de vencimiento
                var schedules = loan.AmortizationSchedules
                    .Where(s => s.AmortizationStatusId != paidStatus.Id && s.DueDate <= DateOnly.FromDateTime(DateTime.Now))
                    .OrderBy(s => s.DueDate).ToList();
                _logger.LogInformation("Iniciando distribución en cascada para el préstamo {LoanId} con monto pagado {AmountPaid}", loan.Id, state.RemainingAmount);

                foreach (var schedule in schedules)
                {
                    if (state.RemainingAmount <= 0) break;

                    #region 1. PRIORIDAD: CUOTA DE MORA
                    var lafee = schedule.LateFees.FirstOrDefault(l => l.LateFeeStatusId != paidFeeStatus.Id);

                    // Si hay una cuota de mora pendiente, aplicar el pago a esa cuota primero
                    if (lafee != null)
                    {
                        // Aplicar el pago a la cuota de mora, pero sin exceder el monto pendiente de esa cuota ni el monto restante del pago
                        var lateFeeAmountToApply = Math.Min((decimal)lafee.Balance, state.RemainingAmount);
                        lafee.Balance = Math.Round(lafee.Balance - (double)lateFeeAmountToApply, 2);
                        state.RemainingAmount -= lateFeeAmountToApply;

                        // Registrar el monto aplicado a mora en el estado de distribución
                        state.TotalAppliedAmount += lateFeeAmountToApply;
                        state.TotalLateFeeAppliedAmount += lateFeeAmountToApply;

                        if (lafee.Balance == 0) lafee.LateFeeStatusId = paidFeeStatus.Id;
                        if (state.RemainingAmount <= 0) break;
                    }
                    #endregion

                    #region 2. PRIORIDAD: INTERÉS DE LA CUOTA
                    decimal interestPending = (decimal)(schedule.InterestAmount - (schedule.InterestPaid ?? 0));

                    if (interestPending > 0)
                    {
                        // Aplicar el pago a los intereses de esta cuota, pero sin exceder el monto pendiente de intereses ni el monto restante del pago
                        var interestToApply = Math.Min(interestPending, state.RemainingAmount);
                        schedule.InterestPaid = Math.Round((schedule.InterestPaid ?? 0) + interestToApply, 2);
                        state.RemainingAmount -= interestToApply;

                        // Registrar el monto aplicado a intereses en el estado de distribución
                        state.TotalAppliedAmount += interestToApply;
                        state.TotalInterestAppliedAmount += interestToApply;

                        if (state.RemainingAmount <= 0) break;
                    }
                    #endregion

                    #region 3. PRIORIDAD: CAPITAL DE LA CUOTA (Lo que pediste)
                    decimal principalPending = (decimal)(schedule.PrincipalAmount - (schedule.PrincipalPaid ?? 0));

                    // Si hay capital pendiente en esta cuota, aplicamos el pago a ese capital, pero sin exceder el monto pendiente de capital ni el monto restante del pago
                    if (principalPending > 0)
                    {
                        var principalToApply = Math.Min(principalPending, state.RemainingAmount);

                        // Actualizamos la cuota
                        schedule.PrincipalPaid = Math.Round((schedule.PrincipalPaid ?? 0) + principalToApply, 2);

                        // Actualizamos el estado para descontar luego del LoanBalance
                        state.TotalPrincipalAppliedAmount += principalToApply;
                        state.TotalAppliedAmount += principalToApply;
                        state.RemainingAmount -= principalToApply;
                    }
                    #endregion

                    // Actualizar el monto pagado total de esta cuota para verificar si se ha pagado completamente
                    schedule.PaidAmount = Math.Round((schedule.PrincipalPaid ?? 0) + (schedule.InterestPaid ?? 0), 2);

                    /// Si esta cuota ya está completamente pagada, marcarla como "Pagada"
                    if (schedule.PaidAmount >= schedule.DueAmount)
                    {
                        schedule.AmortizationStatusId = paidStatus.Id;
                    }
                    else
                    {
                        // Si no se pudo pagar completamente esta cuota, no hay necesidad de seguir aplicando a cuotas futuras
                        state.ApplyExtraToPrincipal = false; // No se aplicará el sobrante a capital, ya que no se cubrió completamente esta cuota
                        break;
                    }
                }

                _logger.LogInformation("Actualizando cuotas de amortización para el préstamo {LoanId} después de la distribución en cascada", loan.Id);
                await _amortizationSchedulesRepository.UpdateManyAsync(schedules);

                if (state.RemainingAmount > 0)
                {
                    state.ApplyExtraToPrincipal = true; // Si quedó un sobrante después de cubrir todas las cuotas, se aplicará a capital
                }

                _logger.LogInformation("Distribución en cascada completada para el préstamo {LoanId} con monto restante {AmountRemaining}", loan.Id, state.RemainingAmount);
                return state;
            }
            catch
            {
                _logger.LogError("Error durante la distribución en cascada para el préstamo {LoanId}", loan.Id);
                throw;
            }
        }

        private async Task HandleExtraPrincipal(PaymentDistributionState state, Domain.Entities.Loan loan)
        {
            try
            {
                _logger.LogInformation("Iniciando reamortización por abono extraordinario para el préstamo {LoanId}", loan.Id);

                var _pendingStatus = await _amortizationStatusRepository.GetByPropertyAsync(s => s.Name == "Pendiente");
                var _paidStatus = await _amortizationStatusRepository.GetByPropertyAsync(s => s.Name == "Pagada");

                // 1. Obtener el balance actual desde la cabecera (LoanBalance)
                var loanBalance = loan.LoanBalance;

                // 2. Calcular el nuevo capital que queda "vivo"
                // Restamos lo que se pagó de capital en las cuotas exigibles + el sobrante que quedó en el state
                decimal newPrincipalBase = Math.Max(0, Math.Round((decimal)loanBalance.PrincipalBalance - (state.TotalPrincipalAppliedAmount + state.RemainingAmount), 2));
                
                // 3. Identificar cuotas futuras a eliminar (solo las "Pendientes")
                var futureSchedules = loan.AmortizationSchedules
                    .Where(s => s.AmortizationStatusId == _pendingStatus.Id && s.DueDate > DateOnly.FromDateTime(DateTime.Now))
                    .OrderBy(s => s.DueDate)
                    .ToList();

                if (newPrincipalBase == 0)
                {
                    _logger.LogInformation("No se requiere reamortización para el préstamo {LoanId} ya que el nuevo capital base es 0", loan.Id);
                    futureSchedules.ForEach(s =>
                    {
                        s.InterestPaid = s.InterestAmount;
                        s.PrincipalPaid = s.PrincipalAmount;
                        s.PaidAmount = s.DueAmount;
                        s.AmortizationStatusId = _paidStatus.Id; // Marcamos como pagadas estas cuotas futuras, ya que no queda capital restante
                    });

                    // Actualizamos estas cuotas para reflejar que se pagaron completamente
                    await _amortizationSchedulesRepository.UpdateManyAsync(futureSchedules);

                    return; // No hay capital restante, por lo que no se necesita reamortizar
                }

                if (futureSchedules.Count > 0)
                {
                    var firstFutureDueDate = futureSchedules.First().DueDate;

                    // Limpiamos las cuotas viejas
                    await _amortizationSchedulesRepository.DeleteManyAsync(futureSchedules);

                    // 4. Llamar al motor de reamortización
                    // Le pasamos la nueva base, la tasa original y la cantidad de cuotas que faltaban
                    var newSchedules = _amortizationService.Calculate(newPrincipalBase, (decimal)loan.InterestRate, futureSchedules.Count, firstFutureDueDate, loan.PaymentFrequency.DaysInterval);

                    // 5. Guardar las nuevas cuotas en la base de datos con estado "Pendiente"
                    List<AmortizationSchedule> paymentSchedule = _mapper.Map<List<AmortizationSchedule>>(newSchedules);
                    paymentSchedule.ForEach(x => x.AmortizationStatusId = _pendingStatus.Id);
                    paymentSchedule.ForEach(x => x.LoanId = loan.Id);

                    await _amortizationSchedulesRepository.AddManyAsync(paymentSchedule);
                    _logger.LogInformation("Reamortización completada para el préstamo {LoanId}. Se generaron {NewScheduleCount} nuevas cuotas a partir de la fecha {FirstDueDate}", loan.Id, paymentSchedule.Count, paymentSchedule.First().DueDate);
                }
            }
            catch
            {
                _logger.LogError("Error durante el proceso de reamortización por abono extraordinario para el préstamo {LoanId}", loan.Id);
                throw;
            }
        }

        private async Task SyncLoanBalances(PaymentDistributionState state, string loanId)
        {
            try
            {
                _logger.LogInformation("Sincronizando balances del préstamo {LoanId} después del pago", loanId);

                // Obtener el préstamo con su balance actual
                var loan = await _loanRepository.GetByIdWithIncludeAsync(l => l.Id == loanId, [loan => loan.LoanBalance]);
                var loanBalance = loan.LoanBalance;

                // Actualizar la fecha del último pago
                loanBalance.LastPaymentDate = DateTime.Now;

                // Actualizar el balance de capital restando lo que se pagó de capital en las cuotas
                loanBalance.PrincipalBalance = Math.Max(0, Math.Round(loanBalance.PrincipalBalance - (double)(state.TotalPrincipalAppliedAmount + state.RemainingAmount), 2));

                // Actualizar el balance de mora restando lo que se pagó de mora en las cuotas
                if (loanBalance.LateFeeBalance > 0)
                {
                    loanBalance.LateFeeBalance = Math.Max(0, Math.Round(loanBalance.LateFeeBalance - (double)state.TotalLateFeeAppliedAmount, 2));

                    if (loanBalance.LateFeeBalance == 0)
                    {
                        var activeStatus = await _loanStatusRepository.GetByPropertyAsync(s => s.Name == LoanStatuses.Active);
                        loan.LoanStatusId = activeStatus.Id;
                        loanBalance.DaysInArrears = 0;
                    }
                }

                // Actualizar el balance pendiente
                loanBalance.TotalOutstanding = Math.Round(loanBalance.PrincipalBalance + loanBalance.InterestBalance + loanBalance.LateFeeBalance, 2);

                // Actualizar el balance cuando se paga el capital completamente
                if (loanBalance.PrincipalBalance <= 0)
                {
                    loanBalance.PrincipalBalance = 0;
                    loanBalance.InterestBalance = 0;
                    loanBalance.LateFeeBalance = 0;
                    loanBalance.TotalOutstanding = 0;
                    loanBalance.DaysInArrears = 0;
                    var paidOffStatus = await _loanStatusRepository.GetByPropertyAsync(s => s.Name == LoanStatuses.Paid);
                    loan.LoanStatusId = paidOffStatus.Id;
                }

                // Guardar los cambios en la base de datos
                await _loanRepository.UpdateAsync(loan);

                _logger.LogInformation("Balances del préstamo {LoanId} actualizados después del pago. Nuevo balance de capital: {PrincipalBalance}, Nuevo balance de intereses: {InterestBalance}, Nuevo balance de mora: {LateFeeBalance}", loan.Id, loanBalance.PrincipalBalance, loanBalance.InterestBalance, loanBalance.LateFeeBalance);
            }
            catch
            {
                _logger.LogError("Error al sincronizar los balances del préstamo {LoanId} después del pago", loanId);
                throw;
            }
        }
    }
}