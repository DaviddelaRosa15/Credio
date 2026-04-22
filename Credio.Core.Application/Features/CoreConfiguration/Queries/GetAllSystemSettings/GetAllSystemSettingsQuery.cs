using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.CoreConfiguration;
using Credio.Core.Application.Enums;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;

namespace Credio.Core.Application.Features.CoreConfiguration.Queries.GetAllSystemSettings
{
    public class GetAllSystemSettingsQuery : IQuery<List<SystemSettingDTO>>
    {
    }

    public class GetAllSystemSettingsQueryHandler : IQueryHandler<GetAllSystemSettingsQuery, List<SystemSettingDTO>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ISystemSettingsRepository _systemSettingsRepository;

        public GetAllSystemSettingsQueryHandler(ICurrentUserService currentUserService, IMapper mapper, ISystemSettingsRepository systemSettingsRepository)
        {
            _currentUserService = currentUserService;
            _mapper = mapper;
            _systemSettingsRepository = systemSettingsRepository;
        }

        public async Task<Result<List<SystemSettingDTO>>> Handle(GetAllSystemSettingsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                List<SystemSettingDTO> response = [];

                // Obtener los roles del usuario actual
                var userRoles = _currentUserService.GetCurrentUserRoles();

                // Si el usuario es SuperAdmin, obtiene todas las configuraciones del sistema
                if (userRoles.Any(x => x == Roles.SuperAdmin.ToString()))
                {
                    var systemSettings = await _systemSettingsRepository.GetAllAsync();
                    response = _mapper.Map<List<SystemSettingDTO>>(systemSettings);
                }
                else
                {
                    // Si el usuario no es SuperAdmin, obtiene solo las configuraciones del sistema que no pertenecen al grupo "EndOfDay"
                    var systemSettings = await _systemSettingsRepository.GetAllByPropertyAsync(s => !s.GroupName.Equals(EndOfDaySettings.GroupName));
                    response = _mapper.Map<List<SystemSettingDTO>>(systemSettings);
                }

                return Result<List<SystemSettingDTO>>.Success(response);
            }
            catch
            {
                return Result<List<SystemSettingDTO>>.Failure(Error.InternalServerError("Hubo un error al obtener las configuraciones del sistema"));
            }
        }
    }
}