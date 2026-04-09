using Credio.Core.Application.Features.Loan.Queries.GetCollectorPortfolioQuery;

namespace Credio.Core.Application.Dtos.Requests;

public class GetCollectorPortfolioRequest
{
    public string? SearchTerm { get; set; }

    public string? State { get; set; }

    public GetCollectorPortfolioQuery ToQuery(string id)
    {
        return new GetCollectorPortfolioQuery
        {
            EmployeeId = id,
            State = State,
            SearchTerm = SearchTerm
        };
    }
}