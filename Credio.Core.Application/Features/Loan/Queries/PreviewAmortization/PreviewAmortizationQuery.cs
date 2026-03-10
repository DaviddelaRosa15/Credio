using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace Credio.Core.Application.Features.Loan.Queries.PreviewAmortization
{
    public sealed class PreviewAmortizationQuery : IQuery<List<InstallmentDTO>>
    {
        public string? LoanApplicationId { get; set; }
        public DateOnly FirstPaymentDate { get; set; }
        /*
        [JsonIgnore]
        [SwaggerIgnore]
        public string CachedKey => $"preview-amortization-{LoanApplicationId}";*/
    }

    public class PreviewAmortizationQueryHandler : IQueryHandler<PreviewAmortizationQuery, List<InstallmentDTO>>
    {
        private readonly IAmortizationCalculatorService _amortizationCalculatorService;
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IMapper _mapper;
        private readonly IPaymentFrequencyRepository _paymentFrequencyRepository;

        public PreviewAmortizationQueryHandler(IAmortizationCalculatorService amortizationService, ILoanApplicationRepository loanApplicationRepository,
            IMapper mapper, IPaymentFrequencyRepository paymentFrequencyRepository)
        {
            _amortizationCalculatorService = amortizationService;
            _loanApplicationRepository = loanApplicationRepository;
            _mapper = mapper;
            _paymentFrequencyRepository = paymentFrequencyRepository;
        }

        public async Task<Result<List<InstallmentDTO>>> Handle(PreviewAmortizationQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var schedule = new List<InstallmentDTO>();

                //Buscar la solicitud de préstamo para verificar su estado y obtener los datos necesarios
                var application = await _loanApplicationRepository.GetByIdWithIncludeAsync(x => x.Id == query.LoanApplicationId,
                [
                    x => x.ApplicationStatus,
                    x => x.PaymentFrequency
                ]);

                if (application is null) return Result<List<InstallmentDTO>>.Failure(Error.NotFound("No se encontro la solicitud con el id dado"));

                if (application.ApplicationStatus.Name != "Aprobada") return Result<List<InstallmentDTO>>.Failure(Error.InternalServerError("La solicitud no está aprobada"));

                schedule = _amortizationCalculatorService.Calculate(
                    (decimal)application.ApprovedAmount.Value,
                    (decimal)application.ApprovedInterestRate.Value,
                    (int)application.ApprovedTerm,
                    query.FirstPaymentDate,
                    application.PaymentFrequency.DaysInterval
                );

                return Result<List<InstallmentDTO>>.Success(schedule);
            }
            catch (Exception ex)
            {
                return Result<List<InstallmentDTO>>.Failure(Error.InternalServerError($"Ocurrio un error inesperado consultando la solicitud"));
            }
        }
    }
}