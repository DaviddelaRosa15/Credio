using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Features.Loan.Queries.GetAllLoans;
using Credio.Core.Application.Features.Loan.Queries.GetDisbursementReceiptPdf;
using Credio.Core.Application.Features.Loan.Queries.GetLoanAmortizationSchedule;
using Credio.Core.Application.Features.Loan.Queries.PreviewAmortization;
using Credio.Core.Application.Features.LoanApplications.Commands.CreateLoan;
using Credio.Core.Application.Features.LoanApplications.Commands.DisburseLoan;
using Credio.Core.Application.Interfaces.Services;
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
    private readonly IPdfService _pdfService;
    private readonly ISender _sender;

    public LoanController(ISender sender, IPdfService pdfService)
    {
        _pdfService = pdfService;
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

    [Authorize(Roles = "Administrator, Officer")]
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoanDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Crea un nuevo prestamo a partir de una solicitud aprobada",
        Description = "Crea un nuevo prestamo a partir de una solicitud aprobada"
    )]
    public async Task<IResult> CreateLoan([FromBody] CreateLoanCommand command, CancellationToken cancellationToken)
    {
        Result<LoanDTO> result = await _sender.Send(command, cancellationToken);

        return result.Match(
          onSuccess: () => CustomResult.Success(result),
          onFailure: CustomResult.Problem);
    }

    [Authorize(Roles = "Administrator, Officer, Collector")]
    [HttpPost("disburse")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DisburseLoanResponseDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Desembolsar prestamo",
        Description = "Desembolsar prestamos creados"
    )]
    public async Task<IResult> DisburseLoan([FromBody] DisburseLoanCommand command, CancellationToken cancellationToken)
    {
        Result<DisburseLoanResponseDTO> result = await _sender.Send(command, cancellationToken);

        return result.Match(
          onSuccess: () => CustomResult.Success(result),
          onFailure: CustomResult.Problem);
    }

    [Authorize(Roles = "Administrator, Officer")]
    [HttpGet("{id}/schedule")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoanAmortizationScheduleResponseDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Obtiene el calendario de pagos del préstamo",
        Description = "Obtiene el calendario de pagos actual del préstamo"
    )]
    public async Task<IResult> GetLoanAmortization(string id, CancellationToken cancellationToken)
    {
        Result<LoanAmortizationScheduleResponseDTO> result = await _sender.Send(new GetLoanAmortizationScheduleQuery { LoanId = id}, cancellationToken);

        return result.Match(
          onSuccess: () => CustomResult.Success(result),
          onFailure: CustomResult.Problem);
    }

    [Authorize(Roles = "Administrator, Officer, Collector")]
    [HttpGet("{id}/disbursement-receipt")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(File))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Obtiene el recibo de desembolso del préstamo",
        Description = "Obtiene el recibo de desembolso actual del préstamo"
    )]
    public async Task<IResult> GetDisbursementReceipt(string id, CancellationToken cancellationToken)
    {
        Result<DisburseLoanNotificationDTO> result = await _sender.Send(new GetDisbursementReceiptPdfQuery { LoanId = id }, cancellationToken);

        if (result.IsSuccess)
        {
            // 1. Obtener los datos
            DisburseLoanNotificationDTO receiptData = result.Value;

            // 2. Generar el PDF en memoria (byte[])
            byte[] pdfBytes = _pdfService.GenerateDisbursementReceipt(receiptData);

            return Results.File(pdfBytes, "application/pdf", $"Recibo_DS_{receiptData.LoanNumber}_{DateTime.Now:yyyyMMdd}.pdf");
        }

        return result.Match(
          onSuccess: () => CustomResult.Success(result),
          onFailure: CustomResult.Problem);
    }

    [Authorize(Roles = "Administrator, Officer, Collector")]
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllLoansQueryResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Obtiene todos los préstamos",
        Description = "Obtiene la lista de todos los préstamos"
    )]
    public async Task<IResult> GetAllLoans([FromQuery] GetAllLoansQuery query, CancellationToken cancellationToken)
    {
        Result<List<GetAllLoansQueryResponse>> result = await _sender.Send(query, cancellationToken);

        return result.Match(
          onSuccess: () => CustomResult.Success(result),
          onFailure: CustomResult.Problem);
    }
}