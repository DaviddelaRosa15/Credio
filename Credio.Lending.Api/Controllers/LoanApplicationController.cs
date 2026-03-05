using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.LoanApplication;
using Credio.Core.Application.Features.LoanApplications.Commands.CreateLoanApplicationCommand;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Lending.Api.Controllers;

[Route("api/v1/loan-application")]
[ApiController]
public class LoanApplicationController : ControllerBase
{
    private readonly ISender _sender;

    public LoanApplicationController(ISender sender)
    {
        _sender = sender;
    }
    
    [SwaggerOperation(
        Summary = "Creacion de solicitud de prestamos",
        Description = "Crear solicitud de prestamo"
    )]
    [Authorize(Roles = "Officer")]
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type =  typeof(LoanApplicationDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IResult> CreateLoanApplication([FromBody] CreateLoanApplicationCommand command, CancellationToken cancellationToken)
    {
        Result<LoanApplicationDto> result = await _sender.Send(command, cancellationToken);

        return result.Match(
            onSuccess: () => CustomResult.Success(result),
            onFailure: CustomResult.Problem);
    }
}