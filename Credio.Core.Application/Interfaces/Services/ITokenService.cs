namespace Credio.Core.Application.Interfaces.Services
{
    public interface ITokenService
    {
        int JwtDurationInMinutes { get; }
        int RefreshDurationInMinutes { get; }

        Task<string> GenerateJWToken(string userId);
		string GenerateRefreshToken(string userId);
		string ValidateRefreshToken();
    }
}