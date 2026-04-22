using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Interfaces.Services;
using Credio.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Credio.Infrastructure.Identity.Services
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
              UserManager<ApplicationUser> userManager,
              SignInManager<ApplicationUser> signInManager,
              IEmailService emailService,
              ITokenService tokenService,
              IHttpContextAccessor httpContextAccessor,
              ILogger<AuthService> logger
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            AuthenticationResponse response = new();
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null)
                {
                    response.HasError = true;
                    response.Error = $"Usuario o contraseña inválidos";
                    return response;
                }

                var isConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                if (!isConfirmed)
                {
                    response.HasError = true;
                    response.Error = "El usuario no ha confirmado su cuenta. Revise su correo electrónico";
                    return response;
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    response.HasError = true;
                    response.Error = $"Usuario o contraseña inválidos";
                    return response;
                }

                response.JWToken = await _tokenService.GenerateJWToken(user.Id);
                response.ExpiresIn = (_tokenService.JwtDurationInMinutes * 60).ToString();
                response.ExpiresAt = DateTime.Now.AddMinutes(_tokenService.JwtDurationInMinutes);
                response.RefreshToken = _tokenService.GenerateRefreshToken(user.Id);
                response.RefreshExpiresIn = (_tokenService.RefreshDurationInMinutes * 60).ToString();
                response.RefreshExpiresAt = DateTime.Now.AddMinutes(_tokenService.RefreshDurationInMinutes);

                _logger.LogInformation("Inicio de sesión finalizado correctamente");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Un error ocurrió tratando de autenticar al usuario");
                throw;
            }
        }
    }
}