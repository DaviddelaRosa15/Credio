using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.LoanApplication;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Credio.Core.Application.Features.LoanApplication
{
    public class GetAllLoanApplicationsQuery : PaginationRequest, ICachedQuery<List<LoanApplicationDto>>
    {
        public string? EmployeeId { get; set; }

        [JsonIgnore]
        [SwaggerIgnore]
        public string CachedKey => $"GetAllLoansQuery__{PageNumber}_{PageSize}_{EmployeeId}";
    }

    public class GetAllLoanApplicationsQueryHandler : IQueryHandler<GetAllLoanApplicationsQuery, List<LoanApplicationDto>>
    {
        private readonly ILoanApplicationRepository _loanRepository;
        private readonly IMapper _mapper;

        public GetAllLoanApplicationsQueryHandler(ILoanApplicationRepository loanRepository, IMapper mapper)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<LoanApplicationDto>>> Handle(GetAllLoanApplicationsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var loans = await _loanRepository.GetPagedAsync(query.PageNumber, query.PageSize,
                    new List<Expression<Func<Domain.Entities.LoanApplication, object>>>
                    {
                        m => m.ApplicationStatus,
                        m => m.Client
                    },
                    !string.IsNullOrWhiteSpace(query.EmployeeId)
                        ? (Expression<Func<Domain.Entities.LoanApplication, bool>>)(c => c.EmployeeId == query.EmployeeId)
                        : null
                    );

                var loansDTOs = _mapper.Map<List<LoanApplicationDto>>(loans.Items);

                return Result<List<LoanApplicationDto>>.Success(loansDTOs);

            }
            catch (Exception ex)
            {
                return Result<List<LoanApplicationDto>>.Failure(Error.InternalServerError("Hubo un error al intentar consultar los préstamos"));
            }
        }
    }
}
