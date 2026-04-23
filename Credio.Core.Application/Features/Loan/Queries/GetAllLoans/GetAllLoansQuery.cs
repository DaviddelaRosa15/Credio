using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace Credio.Core.Application.Features.Loan.Queries.GetAllLoans
{
    public sealed class GetAllLoansQuery : ICachedQuery<List<GetAllLoansQueryResponse>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        [JsonIgnore]
        [SwaggerIgnore]
        public string CachedKey => $"loan-all-{PageNumber}-{PageSize}";
    }

    public class GetAllLoansQueryHandler : IQueryHandler<GetAllLoansQuery, List<GetAllLoansQueryResponse>>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IMapper _mapper;

        public GetAllLoansQueryHandler(ILoanRepository loanRepository, IMapper mapper)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllLoansQueryResponse>>> Handle(GetAllLoansQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var loans = await _loanRepository.GetPagedAsync(query.PageNumber, query.PageSize,
                [x => x.Client, x => x.LoanStatus, x => x.PaymentFrequency]);

                // Mappeamos el préstamo a la respuesta DTO
                var response = _mapper.Map<List<GetAllLoansQueryResponse>>(loans.Items);
                
                return Result<List<GetAllLoansQueryResponse>>.Success(response);
            }
            catch
            {
                return Result<List<GetAllLoansQueryResponse>>.Failure(Error.InternalServerError($"Ocurrio un error inesperado consultando los préstamos"));
            }
        }
    }
}