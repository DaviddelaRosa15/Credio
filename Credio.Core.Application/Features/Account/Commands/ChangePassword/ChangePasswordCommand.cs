using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Dtos.Email;
using Credio.Core.Application.Interfaces.Helpers;
using Credio.Core.Application.Interfaces.Services;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Credio.Core.Application.Features.Account.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<ResetPasswordResponse>
	{
        [SwaggerParameter(Description = "Nueva contraseña")]
        [Required(ErrorMessage = "Debe de ingresar la nueva contraseña para su usuario")]
        public string NewPassword { get; set; }
    }

	public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ResetPasswordResponse>
	{
		private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;
        private readonly IEmailHelper _emailHelper;

        public ChangePasswordCommandHandler(IAccountService accountService, IEmailService emailService, IEmailHelper emailHelper)
		{
			_accountService = accountService;
			_emailService = emailService;
			_emailHelper = emailHelper;
		}


		public async Task<ResetPasswordResponse> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
		{
			ResetPasswordResponse response = new();
			try
			{
				response = await _accountService.ChangePasswordAsync(command.NewPassword);

                await _emailService.SendAsync(new EmailRequest()
                {
                    To = response.Email,
                    Body = _emailHelper.MakeEmailForChange(response.FullName),
                    Subject = "¡Cambio de Contraseña Exitoso - ARS Credio!"
                });

                return response;
			}
			catch (Exception ex)
			{
				response.HasError = true;
				response.Error = "Ocurrió un error.";
				return response;
			}
		}

	}
}
