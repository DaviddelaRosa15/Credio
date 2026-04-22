using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Payment;
using Credio.Core.Application.Features.Payment.Commands.RegisterPayment;
using Credio.Core.Application.Features.Payment.Queries.GetPaymentSearch;
using Credio.Core.Application.Features.Payments.Queries.GetPaymentHistory;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Lending.Api.Controllers;

[Route("api/v1/payments")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly ISender _sender;

    public PaymentController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = "Collector, Officer, Administrator")]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterPaymentResponseDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Registrar un pago realizado por un cliente",
        Description = "Registro de un pago realizado por un cliente, incluyendo detalles como el monto pagado, método de pago, ubicación GPS (opcional), y asociación con un préstamo específico."
    )]
    public async Task<IResult> RegisterPayment([FromBody] RegisterPaymentCommand command, CancellationToken cancellationToken)
    {
        Result<RegisterPaymentResponseDTO> result = await _sender.Send(command, cancellationToken);

        return result.Match(
          onSuccess: () => CustomResult.Success(result),
          onFailure: CustomResult.Problem);
    }

    [Authorize(Roles = "Collector, Officer, Administrator")]
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PaymentSearchDTO>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Buscar cuotas pendientes",
        Description = "Búsqueda de cuotas pendientes de pago para que el usuario pueda visualizar y gestionar los pagos pendientes."
    )]
    public async Task<IResult> SearchPendingPayments([FromQuery] GetPaymentSearchQuery query, CancellationToken cancellationToken)
    {
        Result<List<PaymentSearchDTO>> result = await _sender.Send(query, cancellationToken);
        return result.Match(
          onSuccess: () => CustomResult.Success(result),
          onFailure: CustomResult.Problem);
    }

    [Authorize(Roles = "Administrator, Officer, Collector")]
    [HttpGet("history")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PaymentHistoryDTO>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Obtiene el historial de pago",
        Description = "Obtiene todo el historial de pago o filtra por empleado"
        )]
    public async Task<IResult> GetPaymentHistory([FromQuery] GetPaymentHistoryQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
          onSuccess: () => CustomResult.Success(result),
          onFailure: CustomResult.Problem);
    }
}