using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Enums;

namespace Credio.Core.Application.Interfaces.Services
{
    public interface IAccountService
	{
		Task<RegisterResponse> RegisterEmployeeAsync(RegisterRequest request, Roles role);
		Task<RegisterResponse> RegisterClientAsync(RegisterRequest request);
        Task<ConfirmEmailResponse> ConfirmEmailAsync(string userId, string token);
		Task<ResetPasswordResponse> ResetPasswordAsync(string email);
		ConfirmCodeResponse ConfirmCode(string code);
		Task<ResetPasswordResponse> ChangePasswordAsync(string password);
		Task<UserDTO> GetUsersById(string id);
        Task<RegisterResponse> EditProfile(UserDTO user);
    }
}