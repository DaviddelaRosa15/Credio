using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Interfaces.Services;
using MediatR;

namespace Credio.Core.Application.Features.Account.Commands.ConfirmEmail
{
    public class ConfirmEmailCommand : IRequest<ConfirmEmailResponse>
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }

	public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ConfirmEmailResponse>
	{
		private readonly IAccountService _accountService;

		public ConfirmEmailCommandHandler(IAccountService accountService)
		{
			_accountService = accountService;
		}


		public async Task<ConfirmEmailResponse> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
		{
			ConfirmEmailResponse response = new();
			try
			{
				response = await _accountService.ConfirmEmailAsync(command.UserId, command.Token);
				return response;
			}
			catch (Exception)
			{
				response.HasError = true;
				response.Error = "Ocurrió un error tratando de confirmar la cuenta para el usuario.";
				return response;
			}
		}

	}
}
