using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Credio.Core.Application.Features.LoanStatus.Queries.GetLoanStatuses
{
    public class GetLoanStatusesQuery : PaginationRequest, ICachedQuery<List<LoanStatusDTO>>
    {
        [JsonIgnore]
        [SwaggerIgnore]
        public string CachedKey => $"GetLoanStatusQuery_{PageNumber}_{PageSize}";
    }

    public class GetLoanStatusesQueryHandler : IQueryHandler<GetLoanStatusesQuery, List<LoanStatusDTO>>
    {
        private readonly ILoanStatusRepository _loanStatusRepository;
        private readonly IMapper _mapper;

        public GetLoanStatusesQueryHandler(ILoanStatusRepository loanRepository, IMapper mapper)
        {
            _loanStatusRepository = loanRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<LoanStatusDTO>>> Handle(GetLoanStatusesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var Loans = await _loanStatusRepository.GetPagedAsync(query.PageNumber, query.PageSize,
                    new List<Expression<Func<Domain.Entities.LoanStatus, object>>>
                    {
                        m => m.Loans
                    }
                );

                var loanDTOs = _mapper.Map<List<LoanStatusDTO>>(Loans.Items);

                return Result<List<LoanStatusDTO>>.Success(loanDTOs);

            }
            catch (Exception ex)
            {
                return Result<List<LoanStatusDTO>>.Failure(Error.InternalServerError("Hubo un error al intentar consultar los registros activos"));
            }
        }
    }
}
