using Credio.Core.Application.Dtos.Email;

namespace Credio.Core.Application.Interfaces.Services
{
    public interface IEmailService
	{
		Task SendAsync(EmailRequest request);
	}
}
