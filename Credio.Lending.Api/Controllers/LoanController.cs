using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Features.Loan.Queries.PreviewAmortization;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Interface.Lending.Controllers;

[Route("api/v1/loan")]
[ApiController]
public class LoanController : ControllerBase
{
    private readonly ISender _sender;

    public LoanController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = "Administrator, Officer")]
    [HttpGet("preview-amortization")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<InstallmentDTO>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Obtiene la previsualizacion del pago de su solicitud aprobada",
        Description = "Obtiene una previsualizacion del pago de su solicitud aprobada"
    )]
    public async Task<IResult> GetPreviewAmortization([FromQuery] PreviewAmortizationQuery query, CancellationToken cancellationToken)
    {
        Result<List<InstallmentDTO>> result = await _sender.Send(query, cancellationToken);

        return result.Match(
          onSuccess: () => CustomResult.Success(result),
          onFailure: CustomResult.Problem);
    }
}