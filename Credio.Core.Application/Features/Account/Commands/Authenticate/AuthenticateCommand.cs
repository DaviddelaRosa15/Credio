using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Helpers;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Services;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

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
		private readonly IAccountService _accountService;
		private readonly IMapper _mapper;

		public AuthenticateCommandHandler(IAccountService accountService, IMapper mapper)
		{
			_accountService = accountService;
			_mapper = mapper;
		}


		public async Task<Result<AuthenticationResponse>> Handle(AuthenticateCommand command, CancellationToken cancellationToken)
		{

			try
			{
                var request = _mapper.Map<AuthenticationRequest>(command);
                var response = await _accountService.AuthenticateAsync(request);

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
