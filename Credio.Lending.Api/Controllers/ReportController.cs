using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Features.Loan.Queries.GetPortfolioReportQuery;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Lending.Api.Controllers;

[Route("/api/v1/report")]
[ApiController]
public class ReportController : ControllerBase
{
  private readonly ISender _sender;

  public ReportController(ISender sender)
  {
    _sender = sender;
  }
  
  [SwaggerOperation(
    Summary = "Obtiene el reporte en cartera",
    Description = "Obtiene el reporte en cartera"
  )]
  [Authorize(Roles = "Administrator, Officer")]
  [HttpGet("portfolio")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PortfolioReportResponseDTO))]
  [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
  [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
  public async Task<IResult> GetPortfolioReport([FromQuery] GetPortfolioReportQuery query, CancellationToken cancellationToken)
  {
    Result<PortfolioReportResponseDTO> result = await _sender.Send(query, cancellationToken);

    return result.Match(
      onSuccess: () => CustomResult.Success(result),
      onFailure: CustomResult.Problem);
  }
}