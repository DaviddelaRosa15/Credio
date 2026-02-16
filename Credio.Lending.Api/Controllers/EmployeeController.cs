using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Features.Employee.Commands.RegisterEmployee;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Lending.Api.Controllers
{
    [Route("api/v1/employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ISender _sender;

        public EmployeeController(ISender sender)
        {
            _sender = sender;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterEmployeeCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RegisterEmployeeCommandResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RegisterEmployeeCommandResponse))]
        [SwaggerOperation(
           Summary = "Registro de empleados",
           Description = "Cree usuarios empleados para usar el sistema"
        )]
        public async Task<IResult> RegisterEmployee([FromForm] RegisterEmployeeCommand command, CancellationToken cancellationToken)
        {
            Result<RegisterEmployeeCommandResponse> result = await _sender.Send(command, cancellationToken);

            return result.Match(
            onSuccess: () => CustomResult.Success(result),
            onFailure: CustomResult.Problem);
        }
    }
}
