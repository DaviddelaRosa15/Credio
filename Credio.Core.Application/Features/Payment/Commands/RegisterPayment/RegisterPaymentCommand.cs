using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Payment;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;

namespace Credio.Core.Application.Features.Payment.Commands.RegisterPayment
{
    public class RegisterPaymentCommand : ICommand<RegisterPaymentResponseDTO>
    {
        public string LoanId { get; set; }
        public string CollectorCode { get; set; }
        public double AmountPaid { get; set; }
        public string PaymentMethodId { get; set; }
        public double? GpsLatitude { get; set; }
        public double? GpsLongitude { get; set; }
    }

    public class RegisterPaymentCommandHandler : ICommandHandler<RegisterPaymentCommand, RegisterPaymentResponseDTO>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IPaymentService _paymentService;

        public RegisterPaymentCommandHandler(
            IEmployeeRepository employeeRepository,
            ILoanRepository loanRepository,
            IPaymentService paymentService)
        {
            _employeeRepository = employeeRepository;
            _loanRepository = loanRepository;
            _paymentService = paymentService;
        }

        public async Task<Result<RegisterPaymentResponseDTO>> Handle(RegisterPaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validar que el prestamo exista
                var loan = await _loanRepository.GetByIdWithIncludeAsync(l => l.Id == request.LoanId, [loan => loan.LoanStatus]);

                // Validar que el prestamo este activo o en mora para permitir registrar pagos
                if (loan == null)
                {
                    return Result<RegisterPaymentResponseDTO>.Failure(Error.NotFound("El prestamo no existe"));
                }
                else if(loan.LoanStatus.Name != "Activo" && loan.LoanStatus.Name != "En mora")
                {
                    return Result<RegisterPaymentResponseDTO>.Failure(Error.BadRequest("El prestamo no se encuentra activo o en mora, no se pueden registrar pagos"));
                }

                // Validar que el codigo del cobrador sea valido
                var employee = await _employeeRepository.GetByPropertyAsync(e => e.EmployeeCode == request.CollectorCode);
                if (employee == null)
                {
                    return Result<RegisterPaymentResponseDTO>.Failure(Error.NotFound("El codigo del cobrador no es valido"));
                }

                // 1. Registrar el pago
                var paymentResult = await _paymentService.RegisterInitialPaymentAsync(request, employee.Id, loan.LoanNumber);

                // 2. Procesar el pago (actualizar saldo del prestamo, actualizar estado del prestamo si es necesario, etc)
                await _paymentService.ProcessPaymentAsync(loan.Id, paymentResult);

                // 3. Obtener el recibo de pago actualizado
                var response = await _paymentService.GetPaymentReceiptSnapshotAsync(paymentResult.Id);

                return Result<RegisterPaymentResponseDTO>.Success(response);
            }
            catch(Exception ex)
            {
                return Result<RegisterPaymentResponseDTO>.Failure(Error.InternalServerError("Ocurrió un error al registrar el pago"));
            }
        }
    }
}