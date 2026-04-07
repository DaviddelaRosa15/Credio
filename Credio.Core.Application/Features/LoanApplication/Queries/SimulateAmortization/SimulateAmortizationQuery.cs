using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.LoanApplication;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;

namespace Credio.Core.Application.Features.LoanApplication.Queries.SimulateAmortization
{
    public sealed class SimulateAmortizationQuery : IQuery<SimulationResponseDTO>
    {
        public double Amount { get; set; }
        public double InterestRate { get; set; }
        public string PaymentFrequencyId { get; set; }
        public int Term { get; set; }
        public DateOnly? FirstPaymentDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }

    public class SimulateAmortizationQueryHandler : IQueryHandler<SimulateAmortizationQuery, SimulationResponseDTO>
    {
        private readonly IAmortizationCalculatorService _amortizationCalculatorService;
        private readonly IMapper _mapper;
        private readonly IPaymentFrequencyRepository _paymentFrequencyRepository;

        public SimulateAmortizationQueryHandler(IAmortizationCalculatorService amortizationService,
            IMapper mapper, IPaymentFrequencyRepository paymentFrequencyRepository)
        {
            _amortizationCalculatorService = amortizationService;
            _mapper = mapper;
            _paymentFrequencyRepository = paymentFrequencyRepository;
        }

        public async Task<Result<SimulationResponseDTO>> Handle(SimulateAmortizationQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var schedule = new SimulationResponseDTO();

                // Validar que la frecuencia de pago exista
                var frequency = await _paymentFrequencyRepository.GetByIdAsync(query.PaymentFrequencyId);

                if (frequency == null)
                {
                    return Result<SimulationResponseDTO>.Failure(Error.NotFound("No se encontro la frecuencia de pago con el id dado"));
                }

                // Realizar el calculo de amortizacion
                var amortizationResult = _amortizationCalculatorService.Calculate((decimal)query.Amount, (decimal)query.InterestRate,
                   query.Term, query.FirstPaymentDate.Value, frequency.DaysInterval);

                schedule.TotalAmount = query.Amount;
                schedule.TotalInterest = (double)amortizationResult.Sum(i => i.InterestAmount);
                schedule.TotalToPay = schedule.TotalAmount + schedule.TotalInterest;
                schedule.InstallmentAmount = (double)amortizationResult.First().DueAmount;
                schedule.Schedule = amortizationResult;
                
                return Result<SimulationResponseDTO>.Success(schedule);
            }
            catch (Exception ex)
            {
                return Result<SimulationResponseDTO>.Failure(Error.InternalServerError($"Ocurrio un error inesperado consultando la solicitud"));
            }
        }
    }
}