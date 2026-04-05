using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.CoreConfiguration;
using Credio.Core.Application.Features.CoreConfiguration.Commands.UpdateSystemSetting;
using Credio.Core.Application.Features.CoreConfiguration.Queries.GetAllSystemSettings;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Lending.Api.Controllers;

[Route("api/v1/settings")]
[ApiController]
public class SettingsController : ControllerBase
{
    private readonly ISender _sender;

    public SettingsController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = "SuperAdmin, Administrator")]
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SystemSettingDTO>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Obtener todas las configuraciones del sistema",
        Description = "Obtiene todas las configuraciones del sistema"
    )]
    public async Task<IResult> GetAllSystemSettings([FromQuery] GetAllSystemSettingsQuery query, CancellationToken cancellationToken)
    {
        Result<List<SystemSettingDTO>> result = await _sender.Send(query, cancellationToken);

        return result.Match(
          onSuccess: () => CustomResult.Success(result),
          onFailure: CustomResult.Problem);
    }

    [Authorize(Roles = "SuperAdmin, Administrator")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SystemSettingDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Actualizar los valores de las configuraciones del sistema",
        Description = "Actualiza los valores de las configuraciones del sistema"
    )]
    public async Task<IResult> UpdateSystemSettings([FromBody] UpdateSystemSettingCommand command, CancellationToken cancellationToken)
    {
        Result<SystemSettingDTO> result = await _sender.Send(command, cancellationToken);

        return result.Match(
          onSuccess: () => CustomResult.Success(result),
          onFailure: CustomResult.Problem);
    }
}