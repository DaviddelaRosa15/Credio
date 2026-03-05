using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Client;
using Credio.Core.Application.Dtos.LoanApplication;
using Credio.Core.Application.Features.Clients.Queries;
using Credio.Core.Application.Features.LoanApplication;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;



namespace Credio.Lending.Api.Controllers
{
    [Route("api/v1/loan-application")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ISender _sender;

        public LoanController(ISender sender)
        {
            _sender = sender;
        }

        [Authorize(Roles = "Administrator, Officer")]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoanApplicationDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [SwaggerOperation(
            Summary = "Obtiene todos los prestamos, o filtra por numero de empleado",
            Description = "Obtiene los préstamos registrados, o filtra por id de empleado"
        )]
        public async Task<IResult> GetAllLoans([FromQuery] GetAllLoanApplicationsQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
              onSuccess: () => CustomResult.Success(result),
              onFailure: CustomResult.Problem);
        }

        [Authorize(Roles = "Administrator, Officer")]
        [HttpGet("by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoanApplicationDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [SwaggerOperation(
            Summary = "Obtiene un prestamo por id",
            Description = "Obtiene un prestamo segun el id"
        )]
        public async Task<IResult> GetLoanById(string id, CancellationToken cancellationToken)
        {
            Result<LoanApplicationDto> result = await _sender.Send(new GetLoanApplicationByIdQuery(id), cancellationToken);

            return result.Match(
              onSuccess: () => CustomResult.Success(result),
              onFailure: CustomResult.Problem);
        }
    }
}
