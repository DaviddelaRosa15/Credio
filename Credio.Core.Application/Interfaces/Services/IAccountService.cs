using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Enums;

namespace Credio.Core.Application.Interfaces.Services
{
    public interface IAccountService
	{
		Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
		Task<RegisterResponse> RegisterEmployeeAsync(RegisterRequest request, Roles role);
		Task<RegisterResponse> RegisterClientAsync(RegisterRequest request, Roles role);
        Task<ConfirmEmailResponse> ConfirmEmailAsync(string userId, string token);
		Task<ResetPasswordResponse> ResetPasswordAsync(string email);
		ConfirmCodeResponse ConfirmCode(string code);
		Task<ResetPasswordResponse> ChangePasswordAsync(string password);
		Task<string> GenerateJWToken(string userId);
		string GenerateRefreshToken(string userId);
		string ValidateRefreshToken();
		Task<UserDTO> GetUsersById(string id);
        Task<RegisterResponse> EditProfile(UserDTO user);

    }
}
