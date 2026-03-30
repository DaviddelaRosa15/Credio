using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Features.AmortizationSchedules.Queries.GetUpcomingInstallmentsQuery;
using Credio.Core.Application.Features.Loan.Queries.GetDashboardMetricsQuery;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Lending.Api.Controllers;

[Route("api/v1/dashboard")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }
    
    
    //[Authorize(Roles =  "Administrator, Officer, Collector")]
    [HttpGet("upcoming-installments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UpcomingInstallmentDTO>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Listar las cuotas que están a punto de vencer",
        Description = "Listar las cuotas que están a punto de vencer"
    )]
    public async Task<IResult> GetUpcomingInstallments([FromQuery] GetUpcomingInstallmentsQuery query, CancellationToken cancellationToken)
    {
        Result<List<UpcomingInstallmentDTO>> result = await _sender.Send(query, cancellationToken);

        return result.Match(
            onSuccess: () => CustomResult.Success(result),
            onFailure: CustomResult.Problem);
    }

    [SwaggerOperation(
        Summary = "Obtiene el calculo de las metricas",
        Description = "Obtiene el calculo de las metricas"
    )]
    //[Authorize(Roles = "Administrator, Officer, Collector")]
    [HttpGet("metrics")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DashboardMetricsDTO))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IResult> GetDashboardMetrics([FromQuery] GetDashboardMetricsQuery query, CancellationToken cancellationToken)
    {
        Result<DashboardMetricsDTO> result = await _sender.Send(query, cancellationToken);

        return result.Match(
            onSuccess: () => CustomResult.Success(result),
            onFailure: CustomResult.Problem);
    }
}