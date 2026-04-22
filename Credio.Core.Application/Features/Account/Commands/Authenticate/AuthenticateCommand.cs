using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Core.Application.Features.Account.Commands.Authenticate
{
    public record AuthenticateCommand : ICommand<AuthenticationResponse>
	{
		[SwaggerParameter(Description = "Nombre de usuario")]
		public string UserName { get; set; }

		[SwaggerParameter(Description = "Contraseña")]
		public string Password { get; set; }
	}

	public class AuthenticateCommandHandler : ICommandHandler<AuthenticateCommand, AuthenticationResponse>
	{
		private readonly IAuthService _authService;
		private readonly IMapper _mapper;

		public AuthenticateCommandHandler(IAuthService authService, IMapper mapper)
		{
			_authService = authService;
			_mapper = mapper;
		}


		public async Task<Result<AuthenticationResponse>> Handle(AuthenticateCommand command, CancellationToken cancellationToken)
		{

			try
			{
                var request = _mapper.Map<AuthenticationRequest>(command);
                var response = await _authService.AuthenticateAsync(request);

                if (response.HasError)
                {
                    return Result<AuthenticationResponse>.Failure(Error.BadRequest(response.Error));
                }

                return Result<AuthenticationResponse>.Success(response);
            }
            catch (Exception ex)
			{
				return Result<AuthenticationResponse>.Failure(Error.InternalServerError("Ocurrió un error tratando de autenticar el usuario."));
            }
        }

	}
}