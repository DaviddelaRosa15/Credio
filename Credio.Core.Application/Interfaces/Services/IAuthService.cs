using Credio.Core.Application.Dtos.Account;

namespace Credio.Core.Application.Interfaces.Services
{
    public interface IAuthService
	{
		Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
    }
}