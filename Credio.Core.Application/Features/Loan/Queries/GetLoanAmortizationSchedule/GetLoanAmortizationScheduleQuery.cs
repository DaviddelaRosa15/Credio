using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace Credio.Core.Application.Features.Loan.Queries.GetLoanAmortizationSchedule
{
    public sealed class GetLoanAmortizationScheduleQuery : ICachedQuery<LoanAmortizationScheduleResponseDTO>
    {
        public string LoanId { get; set; } = string.Empty;

        [JsonIgnore]
        [SwaggerIgnore]
        public string CachedKey => $"loan-amortization-{LoanId}";
    }

    public class GetLoanAmortizationScheduleQueryHandler : IQueryHandler<GetLoanAmortizationScheduleQuery, LoanAmortizationScheduleResponseDTO>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IMapper _mapper;

        public GetLoanAmortizationScheduleQueryHandler(ILoanRepository loanRepository,
            IMapper mapper, IPaymentFrequencyRepository paymentFrequencyRepository)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
        }

        public async Task<Result<LoanAmortizationScheduleResponseDTO>> Handle(GetLoanAmortizationScheduleQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var result = new LoanAmortizationScheduleResponseDTO();

                // Consultamos el préstamo con su respectivo cronograma de amortización
                var loan = await _loanRepository.GetByIdWithIncludeAsync(x => x.Id == query.LoanId,
                [
                    x => x.AmortizationSchedules,
                    x => x.Client
                ]);

                // Si no se encuentra el préstamo, retornamos un error de not found
                if (loan is null) return Result<LoanAmortizationScheduleResponseDTO>.Failure(Error.NotFound("No se encontro el préstamo con el id dado"));

                // Mappeamos el préstamo a la respuesta DTO
                result = _mapper.Map<LoanAmortizationScheduleResponseDTO>(loan);
                
                return Result<LoanAmortizationScheduleResponseDTO>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<LoanAmortizationScheduleResponseDTO>.Failure(Error.InternalServerError($"Ocurrio un error inesperado consultando el préstamo"));
            }
        }
    }
}