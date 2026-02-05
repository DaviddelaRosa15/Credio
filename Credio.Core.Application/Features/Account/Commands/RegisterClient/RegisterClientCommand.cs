using AutoMapper;
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
                RegisterResponse response = new();
                /*Lógica para implementar guardado*/

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
