using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.CoreConfiguration;
using Credio.Core.Application.Enums;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Core.Application.Features.CoreConfiguration.Commands.UpdateSystemSetting;

public class UpdateSystemSettingCommand : ICommand<SystemSettingDTO>
{
    [SwaggerParameter(Description = "Identificador de la configuración")]
    public string Key { get; set; }
    
    [SwaggerParameter(Description = "Valor de la configuración")]
    public string Value { get; set; } = string.Empty;
}

public class UpdateSystemSettingCommandHandler : ICommandHandler<UpdateSystemSettingCommand, SystemSettingDTO>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ISystemSettingsRepository _systemSettingsRepository;

    public UpdateSystemSettingCommandHandler(ICurrentUserService currentUserService, IMapper mapper, ISystemSettingsRepository systemSettingsRepository)
    {
        _currentUserService = currentUserService;
        _mapper = mapper;
        _systemSettingsRepository = systemSettingsRepository;
    }

    public async Task<Result<SystemSettingDTO>> Handle(UpdateSystemSettingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener los roles del usuario actual
            var userRoles = _currentUserService.GetCurrentUserRoles();

            // Verificar si la configuración existe
            var systemSetting = await _systemSettingsRepository.GetByPropertyAsync(s => s.Key.Equals(request.Key));

            // Si no existe, retornar error
            if (systemSetting == null) return Result<SystemSettingDTO>.Failure(Error.NotFound("Esa configuración no existe"));

            // Verificar si la configuración es editable
            if ((bool)!systemSetting.IsEditable) return Result<SystemSettingDTO>.Failure(Error.BadRequest("Esa configuración no se puede editar"));

            // Verificar si la configuración pertenece al grupo EndOfDay y el usuario no es SuperAdmin
            if (systemSetting.GroupName.Equals(EndOfDaySettings.GroupName) && !userRoles.Any(u => u.Equals(Roles.SuperAdmin.ToString())))
                return Result<SystemSettingDTO>.Failure(Error.Forbidden("No tienes permisos para editar esta configuración"));

            // Actualizar el valor de la configuración
            systemSetting.Value = request.Value;

            // Guardar los cambios en la base de datos
            await _systemSettingsRepository.UpdateAsync(systemSetting);

            // Mapear la entidad actualizada a un DTO
            var result = _mapper.Map<SystemSettingDTO>(systemSetting);

            return Result<SystemSettingDTO>.Success(result);
        }
        catch 
        {
            return Result<SystemSettingDTO>.Failure(Error.InternalServerError("Ocurrió un error al intentar crear el préstamo"));
        }
    }
}