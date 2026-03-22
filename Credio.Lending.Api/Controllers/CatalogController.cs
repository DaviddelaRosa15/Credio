using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Features.LoanApplications.Commands.CreateLoan;
using Credio.Core.Application.Features.LoanStatus.Queries.GetLoanStatuses;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Lending.Api.Controllers
{
    [Route("/api/v1/catalog")]
    [ApiController]
    public class CatalogController : ControllerBase
    {

        private readonly ISender _sender;

        public CatalogController(ISender sender)
        {
            _sender = sender;
        }

        [SwaggerOperation(
        Summary = "Obtiene los estados de los prestamos",
        Description = "Obtiene los prestamos activos"
        )]
        [Authorize(Roles = "Administrator, Officer")]
        [HttpGet("loan-statuses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoanStatusDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetAllStatusQuery([FromQuery] GetLoanStatusesQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
              onSuccess: () => CustomResult.Success(result),
              onFailure: CustomResult.Problem);
        }
    }
}
