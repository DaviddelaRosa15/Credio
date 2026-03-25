using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Features.AmortizationSchedules.Queries.GetUpcomingInstallmentsQuery;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Lending.Api.Controllers;

[Route("api/v1/dashboard")]
[ApiController]
public class DashBoardController : ControllerBase
{
    private readonly ISender _sender;

    public DashBoardController(ISender sender)
    {
        _sender = sender;
    }
    
    [Authorize(Roles =  "Administrator, Officer")]
    [HttpGet("upcoming-installments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
}