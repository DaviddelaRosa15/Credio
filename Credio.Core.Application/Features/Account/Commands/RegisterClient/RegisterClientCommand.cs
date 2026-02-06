using AutoMapper;
using Avalanche.Core.Application.Helpers;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.Email;
using Credio.Core.Application.Interfaces.Helpers;
using Credio.Core.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Credio.Core.Application.Features.Account.Commands.RegisterClient
{
    public class RegisterClientCommand : IRequest<RegisterResponse>
    {
        [SwaggerParameter(Description = "Nombre")]
        [Required(ErrorMessage = "Debe de ingresar su nombre")]
        public string FirstName { get; set; }

        [SwaggerParameter(Description = "Apellido")]
        [Required(ErrorMessage = "Debe de ingresar su apellido")]
        public string LastName { get; set; }

        [SwaggerParameter(Description = "Tipo de documento")]
        [Required(ErrorMessage = "Debe de ingresar su tipo de documento")]
        public string DocumentType { get; set; }

        [SwaggerParameter(Description = "Número de documento")]
        [Required(ErrorMessage = "Debe de ingresar su número de documento")]
        public string DocumentNumber { get; set; }

        [SwaggerParameter(Description = "Teléfono")]
        [Required(ErrorMessage = "Debe de ingresar su telefono")]
        public string PhoneNumber { get; set; }

        [SwaggerParameter(Description = "Correo")]
        [Required(ErrorMessage = "Debe de ingresar su correo")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido")]
        public string Email { get; set; }

        [SwaggerParameter(Description = "Dirección")]
        [Required(ErrorMessage = "Debe de ingresar su dirección")]
        public AddressDTO Address { get; set; }

        [SwaggerParameter(Description = "Foto de perfil")]
        public IFormFile? Image { get; set; }
    }

    public class RegisterClientCommandHandler : IRequestHandler<RegisterClientCommand, RegisterResponse>
    {
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;
        private readonly IEmailHelper _emailHelper;
        private readonly IMapper _mapper;

        public RegisterClientCommandHandler(IAccountService accountService, IEmailService emailService, IEmailHelper emailHelper,
            IMapper mapper)
        {
            _accountService = accountService;
            _emailService = emailService;
            _emailHelper = emailHelper;
            _mapper = mapper;
        }


        public async Task<RegisterResponse> Handle(RegisterClientCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var request = _mapper.Map<RegisterRequest>(command);
                if (command.Image != null)
                {
                    request.UrlImage = ImageUpload.UploadImage(command.Image, StorageConstants.Clients);
                }
                else
                {
                    request.UrlImage = "";
                }
                request.Address = $"{command.Address.City} {command.Address.AddressLine1} {command.Address.AddressLine2}";
                request.UserName = command.DocumentNumber;
                request.Password = "Credio@" + Guid.NewGuid().ToString().Substring(0, 4);

                var response = await _accountService.RegisterClientAsync(request);

                if (response.Status == "Fallido")
                {
                    ImageUpload.DeleteFile(request.UrlImage);
                    return response;
                }

                /*Envío de correo electrónico
                try
                {
                    UserWelcomeEmail dto = new()
                    {
                        FullName = response.FirstName + " " + response.LastName,
                        UserName = request.UserName,
                        Password = request.Password
                    };

                    await _emailService.SendAsync(new EmailRequest()
                    {
                        To = response.Email,
                        Body = _emailHelper.MakeEmailForAdmin(dto),
                        Subject = "¡Bienvenido/a como Administrador en Avalanche!"
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception("Hubo un error enviando el correo al administrador");
                }*/

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}