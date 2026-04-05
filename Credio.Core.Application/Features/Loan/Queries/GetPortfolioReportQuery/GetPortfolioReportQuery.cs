using System.Text.Json.Serialization;
using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Core.Application.Features.Loan.Queries.GetPortfolioReportQuery;

public class GetPortfolioReportQuery : PaginationRequest, ICachedQuery<PortfolioReportResponseDTO>
{
    public string? SearchTerm { get; set; }

    public string? StatusId { get; set; }

    public DateOnly? StartDate { get; set; }
    
    public DateOnly? EndDate { get; set; }
    
    [JsonIgnore]
    [SwaggerIgnore]
    public string CachedKey => $"GetPortfolioReportQuery_{SearchTerm}_{StatusId}_{PageNumber}_{PageSize}";
}

public class GetPortfolioReportQueryHandler : IQueryHandler<GetPortfolioReportQuery, PortfolioReportResponseDTO>
{
    private readonly IMapper _mapper;
    private readonly ILoanRepository _loanRepository;

    public GetPortfolioReportQueryHandler(
        IMapper mapper,
        ILoanRepository loanRepository)
    {
        _mapper = mapper;
        _loanRepository = loanRepository;
    }
    public async Task<Result<PortfolioReportResponseDTO>> Handle(GetPortfolioReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            PortfolioSummaryDto? summary = await _loanRepository.GetPortfolioSummary(request.StatusId,
                request.SearchTerm,
                request.StartDate,
                request.EndDate,
                cancellationToken);
            
            PagedResult<Domain.Entities.Loan> data = await _loanRepository.GetPagedAsync(
                request.PageNumber, 
                request.PageSize,
                include: query => 
                    query.Include(x => x.Client)
                         .Include(x => x.LoanStatus)
                         .Include(x => x.LoanBalances)
                         .Include(x => x.AmortizationSchedules)
                            .ThenInclude(x => x.AmortizationStatus),
                predicate =>
                        (string.IsNullOrEmpty(request.StatusId) || predicate.LoanStatusId == request.StatusId) &&
                        (!request.StartDate.HasValue || predicate.DisbursedDate >= request.StartDate.Value) &&
                        (!request.EndDate.HasValue || predicate.EffectiveDate <= request.EndDate.Value) &&
                        (
                            string.IsNullOrEmpty(request.SearchTerm) ||
                            predicate.Client.FirstName.Contains(request.SearchTerm) ||
                            predicate.LoanNumber.ToString().Contains(request.SearchTerm)
                        ),
                querySplit: true);
        
            List<LoanReportItemDto> items = _mapper.Map<List<LoanReportItemDto>>(data.Items);
            
            return Result<PortfolioReportResponseDTO>.Success(new PortfolioReportResponseDTO
            {
                Summary = summary,
                Data =  items
            });
        }
        catch
        {
            return Result<PortfolioReportResponseDTO>.Failure(Error.InternalServerError("Hubo un error al intentar consultar los préstamos"));
        }
    }
}