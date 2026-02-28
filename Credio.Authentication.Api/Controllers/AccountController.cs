using Credio.Authentication.Api.Common;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Features.Account.Commands.Authenticate;
using Credio.Core.Application.Features.Account.Commands.ChangePassword;
using Credio.Core.Application.Features.Account.Commands.ConfirmCode;
using Credio.Core.Application.Features.Account.Commands.ConfirmEmail;
using Credio.Core.Application.Features.Account.Commands.RegisterClient;
using Credio.Core.Application.Features.Account.Commands.ResetPassword;
using Credio.Core.Application.Features.Account.Queries.GetRefreshAccessToken;
using Credio.Core.Application.Features.Account.Queries.GetValidationRefreshToken;
using Credio.Core.Application.Helpers;
using Credio.Core.Domain.Settings;
using Credio.Interface.Authentication.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace Credio.Interface.Authentication.Controllers
{
    [ApiController]
    [Route("api/v1/account")]
    [SwaggerTag("Sistema de membresia")]
    public class AccountController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        private readonly ISender _sender;

        private readonly IHostEnvironment env;

        private readonly RefreshJWTSettings _refreshSettings;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IHostEnvironment hostEnvironment, IOptions<RefreshJWTSettings> refreshSettings, ILogger<AccountController> logger, ISender sender)
        {
            env = hostEnvironment;
            _refreshSettings = refreshSettings.Value;
            _logger = logger;
            _sender = sender;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthenticationResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(AuthenticationResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(AuthenticationResponse))]
        [SwaggerOperation(
           Summary = "Iniciar sesión",
           Description = "Autentica al usuario y devuelve un JWT Token"
        )]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IResult> Authenticate([FromBody] AuthenticateCommand command, CancellationToken cancellationToken)
        {
            /*
            string jsonString = JsonSerializer.Serialize(command, new JsonSerializerOptions
            {
                WriteIndented = true // Optional: for pretty-printing
            });

            // Log the JSON string
            _logger.LogInformation("Object as JSON: {JsonString}", jsonString);
            var response = await Mediator.Send(command);
            */

            Result<AuthenticationResponse> result = await _sender.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.Now.AddMinutes(_refreshSettings.DurationInMinutes),
                    SameSite = SameSiteMode.None
                });
            }

            return result.Match(
            onSuccess: () => CustomResult.Success(result),
            onFailure: CustomResult.Problem);
        }

        [HttpGet("refresh-access-token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(AuthenticationResponse))]
        [SwaggerOperation(
           Summary = "Obtener nuevo access token",
           Description = "Valida el refresh token y devuelve un JWT Token de acceso nuevo"
        )]
        public async Task<IActionResult> RefreshAccesToken()
        {
            try
            {
                var response = await Mediator.Send(new GetRefreshAccessTokenQuery());

                if (response.HasError)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ErrorMapperHelper.Error(ErrorMessages.InternalServer, response.Error));
                }

                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorMapperHelper.Error(ErrorMessages.InternalServer, e.Message));
            }
        }

        //[Authorize(Roles = "Administrator")]
        [HttpPost("register-client")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RegisterResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RegisterResponse))]
        [SwaggerOperation(
           Summary = "Registro de clientes",
           Description = "Cree usuarios clientes para hacer las solicitudes o los préstamos"
        )]
        public async Task<IActionResult> RegisterClient([FromForm] RegisterClientCommand command)
        {
            try
            {
                var response = await Mediator.Send(command);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ErrorMapperHelper.ListError(errors));
                }

                if (response.Status == "Fallido")
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorMapperHelper.Error(ErrorMessages.InternalServer, e.Message));
            }
        }

        [HttpGet("validate-refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetValidationRefreshTokenQueryResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(AuthenticationResponse))]
        [SwaggerOperation(
           Summary = "Validar el refresh token",
           Description = "Verifica si existe refresh token y lo valida"
        )]
        public async Task<IActionResult> ValidateRefreshToken()
        {
            try
            {
                var response = await Mediator.Send(new GetValidationRefreshTokenQuery());

                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorMapperHelper.Error(ErrorMessages.InternalServer, e.Message));
            }
        }

        [HttpGet("confirm-email")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ConfirmEmailResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ConfirmEmailResponse))]
        [SwaggerOperation(
           Summary = "Comfirmar al usuario ",
           Description = "Confirma la cuenta del usuario"
        )]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                ConfirmEmailCommand command = new()
                {
                    UserId = userId,
                    Token = token
                };
                var response = await Mediator.Send(command);

                if (response.HasError)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ErrorMapperHelper.Error(ErrorMessages.InternalServer, response.Error));
                }

                return RedirectToAction("thanks", new { name = response.NameUser });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorMapperHelper.Error(ErrorMessages.InternalServer, e.Message));
            }
        }

        [HttpGet("thanks")]
        public IActionResult Thanks(string name)
        {
            string htmlBody = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Bienvenido/a al Sistema</title>
    <style>
        /* Estilos adicionales */
        body {
            font-family: Arial, sans-serif;
        }
        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f8f8f8;
            border-radius: 10px;
        }
        .header {
            text-align: center;
            margin-bottom: 20px;
        }
        .title {
            font-size: 24px;
            margin-bottom: 10px;
        }
        .message {
            font-size: 16px;
            margin-bottom: 20px;
        }
        .button {
            display: inline-block;
            font-weight: 400;
            text-align: center;
            white-space: nowrap;
            vertical-align: middle;
            user-select: none;
            border: 1px solid transparent;
            padding: 0.375rem 0.75rem;
            font-size: 1rem;
            line-height: 1.5;
            border-radius: 0.25rem;
            transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
            color: #fff;
            background-color: #007bff;
            border-color: #007bff;
            text-decoration: none;
        }
        .footer {
            text-align: center;
            font-size: 14px;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 class='title'>¡Bienvenido/a al Sistema!</h1>
        </div>
        <div class='message'>
            <p>Hola [Nombre],</p>
            <p>Gracias por confirmar tu cuenta. Ahora tienes acceso completo a todas las funcionalidades del sistema.</p>
            <p>Disfruta de todas las características y no dudes en ponerte en contacto con nosotros si tienes alguna pregunta o necesitas asistencia.</p>
            <p><a href='http://localhost:5173/' class='button'>Acceder al Sistema</a></p>
        </div>
        <div class='footer'>
            <p>Atentamente,</p>
            <p>El equipo de Base</p>
        </div>
    </div>
</body>
</html>
";

            string html = htmlBody.Replace("[Nombre]", name);
            return Content(html, "text/html");
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResetPasswordResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResetPasswordResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResetPasswordResponse))]
        [SwaggerOperation(
            Summary = "Restablecer contraseña",
            Description = "Permite que el usuario cambie su contraseña si se le olvidó"
            )]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            try
            {
                var response = await Mediator.Send(command);

                if (response.HasError)
                {
                    if (response.Error.Contains("error"))
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, ErrorMapperHelper.Error(ErrorMessages.InternalServer, response.Error));
                    }
                    return NotFound(ErrorMapperHelper.Error(ErrorMessages.NotFound, response.Error));
                }

                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorMapperHelper.Error(ErrorMessages.InternalServer, e.Message));
            }
        }

        [HttpPost("confirm-code")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConfirmCodeResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ConfirmCodeResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ConfirmCodeResponse))]
        [SwaggerOperation(
            Summary = "Confirmar código",
            Description = "Permite que el usuario ingrese el código de confirmación que se le envió por correo"
            )]
        public async Task<IActionResult> ConfirmCode([FromBody] ConfirmCodeCommand command)
        {
            try
            {
                var response = await Mediator.Send(command);

                if (response.HasError)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ErrorMapperHelper.Error(ErrorMessages.InternalServer, response.Error));
                }

                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorMapperHelper.Error(ErrorMessages.InternalServer, e.Message));
            }
        }

        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResetPasswordResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResetPasswordResponse))]
        [SwaggerOperation(
            Summary = "Restablecer contraseña",
            Description = "Permite que el usuario cambie su contraseña si se le olvidó"
            )]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            try
            {
                var response = await Mediator.Send(command);

                if (response.HasError)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ErrorMapperHelper.Error(ErrorMessages.InternalServer, response.Error));
                }

                HttpContext.Session.Remove("user");
                HttpContext.Session.Remove("confirmCode");

                return Ok(response.IsSuccess);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorMapperHelper.Error(ErrorMessages.InternalServer, e.Message));
            }
        }

        [HttpGet("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
           Summary = "Salir de sesión",
           Description = "Borra el refresh token"
        )]
        public async Task<IActionResult> Logout()
        {
            try
            {
                Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.Now.AddMinutes(_refreshSettings.DurationInMinutes),
                    SameSite = SameSiteMode.None
                });

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorMapperHelper.Error(ErrorMessages.InternalServer, e.Message));
            }
        }
    }
}