using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Client;
using Credio.Core.Application.Dtos.Employee;
using Credio.Core.Application.Features.Clients.Queries;
using Credio.Core.Application.Features.Employee.Commands.RegisterEmployee;
using Credio.Core.Application.Features.Employee.Queries.GetAll;
using Credio.Core.Application.Features.Employee.Queries.GetEmployeeByCode;
using Credio.Core.Application.Features.Employee.Queries.GetEmployeeById;
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
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
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

        [Authorize(Roles = "Administrator")]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [SwaggerOperation(
           Summary = "Consulta de empleados",
           Description = "Consulta todos los empleados con paginación"
        )]
        public async Task<IResult> GetAllEmployee([FromQuery] GetAllEmployeeQuery query, CancellationToken cancellationToken)
        {
            Result<List<EmployeeDTO>> result = await _sender.Send(query, cancellationToken);

            return result.Match(
            onSuccess: () => CustomResult.Success(result),
            onFailure: CustomResult.Problem);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("by-code/{employeeCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeDetailDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(EmployeeDetailDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [SwaggerOperation(
            Summary = "Obtiene el empleado por codigo de empleado",
            Description = "Obtiene al empleado segun el codigo dado"
        )]
        public async Task<IResult> GetEmployeeByCode(string EmployeeCode, CancellationToken cancellationToken)
        {
            Result<EmployeeDetailDTO> result = await _sender.Send(new GetEmployeeByCodeQuery(EmployeeCode), cancellationToken);

            return result.Match(
              onSuccess: () => CustomResult.Success(result),
              onFailure: CustomResult.Problem);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeDetailDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(EmployeeDetailDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [SwaggerOperation(
            Summary = "Obtiene el empleado por id",
            Description = "Obtiene al empleado segun el id dado"
        )]
        public async Task<IResult> GetEmployeeById(string id, CancellationToken cancellationToken)
        {
            Result<EmployeeDetailDTO> result = await _sender.Send(new GetEmployeeByIdQuery(id), cancellationToken);

            return result.Match(
              onSuccess: () => CustomResult.Success(result),
              onFailure: CustomResult.Problem);
        }
    }
}
