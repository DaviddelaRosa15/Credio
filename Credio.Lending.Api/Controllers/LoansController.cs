using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Client;
using Credio.Core.Application.Dtos.LoanApplication;
using Credio.Core.Application.Features.LoanApplication;
using Credio.Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;

namespace Credio.Lending.Api.Controllers
{
    [Route("api/v1/loan-application")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ISender _sender;

        public LoansController(ISender sender)
        {
            _sender = sender;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoanApplicationDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [SwaggerOperation(
            Summary = "Obtiene todos los prestamos, o filtra por numero de empleado",
            Description = "Obtiene los préstamos registrados, o filtra por id de empleado"
        )]
        public async Task<IResult> GetAllLoan([FromQuery] GetAllLoanApplicationsQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
              onSuccess: () => CustomResult.Success(result),
              onFailure: CustomResult.Problem);
        }
    }
}
