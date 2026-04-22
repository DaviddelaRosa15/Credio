using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Settings;
using Credio.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Credio.Infrastructure.Identity.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWTSettings _jwtSettings;
        private readonly RefreshJWTSettings _refreshSettings;
        IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
              UserManager<ApplicationUser> userManager,
              IOptions<JWTSettings> jwtSettings,
              IOptions<RefreshJWTSettings> refreshSettings,
              IHttpContextAccessor httpContextAccessor,
              ILogger<TokenService> logger
            )
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _refreshSettings = refreshSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // Propiedades para acceder a la duración de los tokens desde la configuración
        public int JwtDurationInMinutes => _jwtSettings.DurationInMinutes;
        public int RefreshDurationInMinutes => _refreshSettings.DurationInMinutes;

        public async Task<string> GenerateJWToken(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("uid", user.Id),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("UrlImage", user.UrlImage)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmectricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredetials = new SigningCredentials(symmectricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredetials);


            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }

        public string GenerateRefreshToken(string userId)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", userId)
            };

            var symmectricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshSettings.Key));
            var signingCredetials = new SigningCredentials(symmectricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _refreshSettings.Issuer,
                audience: _refreshSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_refreshSettings.DurationInMinutes),
                signingCredentials: signingCredetials);

            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }

        public string ValidateRefreshToken()
        {
            string token = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
            if (token == null)
            {
                return "Error: No existen token de actualización";
            }

            string userId = "";
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = _refreshSettings.Issuer,
                ValidAudience = _refreshSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshSettings.Key))
            };

            try
            {
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                if (validatedToken == null)
                {
                    return "Error: El token no es válido";
                }
                var id = claimsPrincipal.FindFirst("uid");
                userId = id.Value;
            }
            catch (SecurityTokenValidationException ex)
            {
                return "Error de validación del token JWT: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Error al decodificar el token JWT: " + ex.Message;
            }

            return userId;
        }
    }
}
