using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Dtos.LoanApplication;
using Credio.Core.Application.Dtos.LoanStatus;
using Credio.Core.Application.Features.Loan.Queries.GetNextPaymentSummaryByDocumentQuery;
using Credio.Core.Application.Features.LoanApplication.Queries.GetApplicationStatusForBotQuery;
using Credio.Core.Application.Features.LoanStatus.Queries.GetLoanStatuses;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Lending.Api.Controllers;

[Route("/api/v1/bot")]
[ApiController]
public class BotController : ControllerBase
{
    private readonly ISender _sender;

    public BotController(ISender sender)
    {
        _sender = sender;
    }

    [SwaggerOperation(
        Summary = "Obtiene los proximos pagos por el documento de identidad",
        Description = "Obtiene los proximos pagos por el documento de identidad"
    )]
    [Authorize(Roles = "Administrator, Officer")]
    [HttpGet("payments/summary")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BotNextPaymentResponseDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IResult> GetNextPaymentSummary([FromQuery] GetNextPaymentSummaryByDocumentQuery query, CancellationToken cancellationToken)
    {
        Result<BotNextPaymentResponseDTO> result = await _sender.Send(query, cancellationToken);

        return result.Match(
            onSuccess: () => CustomResult.Success(result),
            onFailure: CustomResult.Problem);
    }
    
    [Authorize(Roles = "Administrator, Officer")]
    [HttpGet("applications/status/{documentNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BotApplicationStatusDTO>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Consulta que devuelva el estatus actual de las solicitudes recientes de un cliente",
        Description = "Consulta que devuelva el estatus actual de las solicitudes recientes de un cliente"
    )]
    public async Task<IResult> GetApplicationStatusByDocumentNumber(string documentNumber, CancellationToken cancellationToken = default)
    {
        Result<List<BotApplicationStatusDTO>> result = await _sender.Send(new GetApplicationStatusForBotQuery{DocumentNumber = documentNumber}, cancellationToken);
        
        return result.Match(
            onSuccess: () => CustomResult.Success(result),
            onFailure:CustomResult.Problem);
    }
}