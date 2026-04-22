using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.Email;
using Credio.Core.Application.Enums;
using Credio.Core.Application.Helpers;
using Credio.Core.Application.Interfaces.Services;
using Credio.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Credio.Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
              UserManager<ApplicationUser> userManager,
              IEmailService emailService,
              IHttpContextAccessor httpContextAccessor,
              ILogger<AccountService> logger
            )
        {
            _userManager = userManager;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<RegisterResponse> RegisterEmployeeAsync(RegisterRequest request, Roles role)
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
                UrlImage = request.UrlImage,
                Address = request.Address,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            try
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    response.Id = user.Id;
                    response.FirstName = user.FirstName;
                    response.LastName = user.LastName;
                    response.Email = user.Email;
                    response.PhoneNumber = user.PhoneNumber;
                    response.Address = user.Address;
                    response.UrlImage = user.UrlImage;

                    await _userManager.AddToRoleAsync(user, role.ToString());
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
                response.Details = [new ErrorDetailsDTO { Code = "000", Message = $"Se insertó correctamente el usuario"}];

                //_logger.LogInformation($"La contraseña del usuario {request.UserName} es {request.Password}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException, "Error al crear usuario");
                response.Status = "Fallido";
                response.Details = [new ErrorDetailsDTO { Code = ErrorMessages.InternalServer, Message = "Ocurrió algo mientras se creaba el usuario" }];
                return response;
            }
        }

        public async Task<RegisterResponse> RegisterClientAsync(RegisterRequest request)
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
                UrlImage = request.UrlImage,
                Address = request.Address,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            try
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    response.Id = user.Id;
                    response.FirstName = user.FirstName;
                    response.LastName = user.LastName;
                    response.Email = user.Email;
                    response.PhoneNumber = user.PhoneNumber;
                    response.Address = user.Address;
                    response.UrlImage = user.UrlImage;

                    await _userManager.AddToRoleAsync(user, Roles.Client.ToString());
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
                response.Details = [new ErrorDetailsDTO { Code = "000", Message = $"Se insertó correctamente el cliente" }];

                //_logger.LogInformation($"La contraseña del usuario {request.UserName} es {request.Password}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException, "Error al crear usuario");
                response.Status = "Fallido";
                response.Details = [new ErrorDetailsDTO { Code = ErrorMessages.InternalServer, Message = "Ocurrió algo mientras se creaba el usuario" }];
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
                UrlImage = user.UrlImage,
                Address = user.Address,
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
            userToUpdate.UrlImage = user.UrlImage;
            userToUpdate.Address = user.Address;

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