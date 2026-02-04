using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.Email;
using Credio.Core.Application.Enums;
using Credio.Core.Application.Helpers;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Settings;
using Credio.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Credio.Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly JWTSettings _jwtSettings;
        private readonly RefreshJWTSettings _refreshSettings;
        IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
              UserManager<ApplicationUser> userManager,
              SignInManager<ApplicationUser> signInManager,
              IEmailService emailService,
              IOptions<JWTSettings> jwtSettings,
              IOptions<RefreshJWTSettings> refreshSettings,
              IHttpContextAccessor httpContextAccessor,
              ILogger<AccountService> logger
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _jwtSettings = jwtSettings.Value;
            _refreshSettings = refreshSettings.Value;
            _httpContextAccessor = httpContextAccessor;
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
                    response.Error = $"No existe una cuenta registrada con este usuario: {request.UserName}";
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

                response.JWToken = await GenerateJWToken(user.Id);
                response.ExpiresIn = (_jwtSettings.DurationInMinutes * 60).ToString();
                response.ExpiresAt = DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes);
                response.RefreshToken = GenerateRefreshToken(user.Id);
                response.RefreshExpiresIn = (_refreshSettings.DurationInMinutes * 60).ToString();
                response.RefreshExpiresAt = DateTime.Now.AddMinutes(_refreshSettings.DurationInMinutes);

                _logger.LogInformation("Inicio de sesión finalizado correctamente");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Un error ocurrió tratando de autenticar al usuario");
                response.HasError = true;
                response.Error = ex.Message;
                return response;
            }
        }

        public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest request, Roles role)
        {
            RegisterResponse response = await ValidateUserBeforeRegistrationAsync(request);

            if (response.Status == "Fallido")
            {
                return response;
            }

            response.Details = new();
            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            try
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                string userRole = "";
                if (result.Succeeded)
                {
                    response.Id = user.Id;
                    response.FirstName = user.FirstName;
                    response.LastName = user.LastName;
                    response.Email = user.Email;
                    response.PhoneNumber = user.PhoneNumber;

                    await _userManager.AddToRoleAsync(user, role.ToString());

                    switch (role)
                    {
                        case Roles.Analyst:
                            userRole = "analista";
                            break;
                        case Roles.Administrator:
                            userRole = "administrador";
                            break;
                        case Roles.Guest:
                            userRole = "hospital";
                            break;
                    }
                }
                else
                {
                    response.Status = "Fallido";
                    foreach (var error in result.Errors)
                    {
                        ErrorDetailsDTO errorDTO = new()
                        {
                            Code = ErrorMessages.BadRequest,
                            Message = error.Description
                        };
                        response.Details.Add(errorDTO);
                    }
                    return response;
                }

                response.Status = "Exitoso";
                response.Details = [new ErrorDetailsDTO { Code = "000", Message = $"Se insertó correctamente el {userRole}"}];

                _logger.LogInformation($"La contraseña del usuario {request.UserName} es {request.Password}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException, "Error al crear usuario");
                response.Status = "Fallido";
                response.Details = [new ErrorDetailsDTO { Code = "000", Message = "Ocurrió algo mientras se creaba el usuario" }];
                return response;
            }
        }

        public async Task<ConfirmEmailResponse> ConfirmEmailAsync(string userId, string token)
        {
            ConfirmEmailResponse response = new()
            {
                HasError = false
            };

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    response.HasError = true;
                    response.Error = "No existe cuenta registrada con este usuario";
                    return response;
                }

                token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    await _emailService.SendAsync(new EmailRequest()
                    {
                        To = user.Email,
                        Body = EmailHelper.MakeEmailForConfirmed(user.FirstName + " " + user.LastName),
                        Subject = "Cuenta Confirmada"
                    });
                    response.NameUser = user.FirstName + " " + user.LastName;

                    _logger.LogInformation("Confirmación de cuenta finalizado correctamente");
                    return response;
                }
                else
                {
                    response.HasError = true;
                    response.Error = $"Ocurrió un error mientras se confirmaba la cuenta para el correo: {user.Email}";
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Un error ocurrió tratando de confirmar el usuario");
                response.HasError = true;
                response.Error = ex.Message;
                return response;
            }
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(string email)
        {
            ResetPasswordResponse response = new()
            {
                HasError = false
            };

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    response.HasError = true;
                    response.Error = "No existe cuenta registrada con este correo";
                    return response;
                }

                Guid guid = Guid.NewGuid();
                string format = guid.ToString();
                string code = format.Replace("-", "");
                code = code.Substring(5, 6);

                _httpContextAccessor.HttpContext.Session.SetString("confirmCode", code);
                _httpContextAccessor.HttpContext.Session.SetString("user", user.Id);

                response.FullName = user.FirstName + " " + user.LastName;
                response.Email = user.Email;
                response.Code = code;
                response.IsSuccess = true;

                _logger.LogInformation("Envío de código finalizado correctamente"); 
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Un error ocurrió tratando de resetear la contraseña");
                response.HasError = true;
                response.Error = ex.Message;
                return response;
            }
        }

        public async Task<ResetPasswordResponse> ChangePasswordAsync(string password)
        {
            ResetPasswordResponse response = new()
            {
                HasError = false
            };

            try
            {
                var userId = _httpContextAccessor.HttpContext.Session.GetString("user");
                if (userId == null)
                {
                    response.HasError = true;
                    response.Error = "Sucedió un error en el sistema";
                    return response;
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    response.HasError = true;
                    response.Error = "Sucedió un error en el sistema";
                    return response;
                }
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(user, resetToken, password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        response.Error += $"{error.Description}";
                    }
                    response.HasError = true;

                    return response;
                }

                response.FullName = user.FirstName + " " + user.LastName;
                response.Email = user.Email;
                response.IsSuccess = true;

                _logger.LogInformation("Restablecimiento de contraseña finalizado correctamente");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Un error ocurrió tratando de crear el usuario");
                response.HasError = true;
                response.Error = ex.Message;
                return response;
            }
        }

        public ConfirmCodeResponse ConfirmCode(string code)
        {
            ConfirmCodeResponse response = new()
            {
                HasError = false
            };

            try
            {
                var confirm = _httpContextAccessor.HttpContext.Session.GetString("confirmCode");
                if (confirm == null)
                {
                    response.HasError = true;
                    response.Error = "Ocurrió un error confirmando el código";
                    return response;
                }

                if (!confirm.Equals(code))
                {
                    response.HasError = true;
                    response.Error = "El código de confirmación no es correcto";
                    return response;
                }

                _logger.LogInformation("Confirmación de código finalizado correctamente");
                response.IsSuccess = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Un error ocurrió tratando de crear el usuario");
                response.HasError = true;
                response.Error = ex.Message;
                return response;
            }
        }

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

        public async Task<UserDTO> GetUsersById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            UserDTO dto = new()
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.First(),
            };

            return dto;
        }

        public async Task<RegisterResponse> EditProfile(UserDTO user)
        {
            RegisterResponse response = new();
            var userToUpdate = await _userManager.FindByIdAsync(user.Id);

            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;

            try
            {
                await _userManager.UpdateAsync(userToUpdate);
            }
            catch (Exception ex)
            {
                throw;
            }

            response.Status = "Exitoso";
            response.Details = [new ErrorDetailsDTO { Code = "000", Message = "Se editó correctamente el perfil" }];
            return response;
        }

        #region Private Methods
        private async Task<RegisterResponse> ValidateUserBeforeRegistrationAsync(RegisterRequest request)
        {
            RegisterResponse response = new();

            try
            {
                var user = _userManager.Users.ToList();

                var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
                if (userWithSameUserName != null)
                {
                    var error = ErrorMapperHelper.Error(ErrorMessages.BadRequest, $"El nombre de usuario '{request.UserName}' ya está siendo usado.");
                    response.Status = error.Status;
                    response.Details = error.Details;
                    return response;
                }

                var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
                if (userWithSameEmail != null)
                {
                    
                    var error = ErrorMapperHelper.Error(ErrorMessages.BadRequest, $"El correo '{request.Email}' ya está siendo usado.");
                    response.Status = error.Status;
                    response.Details = error.Details;
                    return response;
                }

                return response;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Un error ocurrió tratando de validar al usuario");
                throw;
            }
            
        }

        private async Task<string> SendVerificationEmailUri(ApplicationUser user)
        {
            var origin = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/v1/Account/confirm-email";
            var Uri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(Uri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "token", code);

            return verificationUri;
        }

        #endregion
    }
}
