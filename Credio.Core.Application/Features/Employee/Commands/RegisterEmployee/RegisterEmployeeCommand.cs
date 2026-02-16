using AutoMapper;
using Avalanche.Core.Application.Helpers;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Enums;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Helpers;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Core.Application.Features.Employee.Commands.RegisterEmployee
{
    public class RegisterEmployeeCommand : ICommand<RegisterEmployeeCommandResponse>
    {
        [SwaggerParameter(Description = "Nombre")]
        public string FirstName { get; set; }

        [SwaggerParameter(Description = "Apellido")]
        public string LastName { get; set; }

        [SwaggerParameter(Description = "Tipo de documento")]
        public string DocumentType { get; set; }

        [SwaggerParameter(Description = "Número de documento")]
        public string DocumentNumber { get; set; }

        [SwaggerParameter(Description = "Teléfono")]
        public string Phone { get; set; }

        [SwaggerParameter(Description = "Correo")]
        public string Email { get; set; }

        [SwaggerParameter(Description = "Rol")]
        public string Role { get; set; }

        [SwaggerParameter(Description = "Dirección")]
        public AddressDTO? Address { get; set; }

        [SwaggerParameter(Description = "Foto de perfil")]
        public IFormFile? Image { get; set; }
    }

    public class RegisterEmployeeCommandHandler : ICommandHandler<RegisterEmployeeCommand, RegisterEmployeeCommandResponse>
    {
        private readonly IAccountService _accountService;
        private readonly IAddressRepository _addressRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;
        private readonly IEmailHelper _emailHelper;
        private readonly IMapper _mapper;

        public RegisterEmployeeCommandHandler(IAccountService accountService, IAddressRepository addressRepository, IEmployeeRepository employeeRepository,
            IDocumentTypeRepository documentTypeRepository, IEmailService emailService, IEmailHelper emailHelper,
            IMapper mapper)
        {
            _accountService = accountService;
            _addressRepository = addressRepository;
            _documentTypeRepository = documentTypeRepository;
            _employeeRepository = employeeRepository;
            _emailService = emailService;
            _emailHelper = emailHelper;
            _mapper = mapper;
        }


        public async Task<Result<RegisterEmployeeCommandResponse>> Handle(RegisterEmployeeCommand command, CancellationToken cancellationToken)
        {
            try
            {
                RegisterEmployeeCommandResponse result = new();
                var employee = _mapper.Map<Domain.Entities.Employee>(command);
                var request = _mapper.Map<RegisterRequest>(command);

                //Obtener el último código de empleado registrado para generar el nuevo código
                var lastEmployeeCode = await _employeeRepository.GetLastEmployeeCodeAsync();
                employee.EmployeeCode = $"U{lastEmployeeCode + 1}";

                //Generar nombre de usuario y contraseña temporal
                request.UserName = employee.EmployeeCode;
                request.Password = "Credio@" + Guid.NewGuid().ToString().Substring(0, 4);
                request.Address = $"{command.Address.City} {command.Address.AddressLine1} {command.Address.AddressLine2}";
                if (command.Image != null)
                {
                    request.UrlImage = ImageUpload.UploadImage(command.Image, StorageConstants.Employees);
                }
                else
                {
                    request.UrlImage = "";
                }

                //Asignar rol
                var role = Roles.Officer;
                switch(command.Role.ToLower())
                {
                    case "administrator":
                        role = Roles.Administrator;
                        break;
                    case "collector":
                        role = Roles.Collector;
                        break;
                    case "officer":
                        role = Roles.Officer;
                        break;
                }

                //Registro de usuario en Identity
                var response = await _accountService.RegisterEmployeeAsync(request, role);
                if (response.Status == "Fallido")
                {
                    ImageUpload.DeleteFile(request.UrlImage);
                    return Result<RegisterEmployeeCommandResponse>.Failure(Error.BadRequest(response.Details[0].Message));
                }
                employee.UserId = response.Id;

                //Registro de dirección
                var employeeAddress = _mapper.Map<Address>(command.Address);
                //var address = await _addressRepository.AddAsync(employeeAddress);
                employee.Address = employeeAddress;

                //Validar que el tipo de documento exista
                var documentType = await _documentTypeRepository.GetByPropertyAsync(d => d.Name == command.DocumentType);
                if (documentType == null)
                {
                    throw new Exception("El tipo de documento proporcionado no existe.");
                }
                employee.DocumentTypeId = documentType.Id;

                var employeeCreated = await _employeeRepository.AddAsync(employee);

                result = _mapper.Map<RegisterEmployeeCommandResponse>(employeeCreated);
                result.DocumentType = command.DocumentType;
                result.UserName = request.UserName;
                result.Address = command.Address;
                result.Role = command.Role;
                result.UrlImage = response.UrlImage;

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

                return Result<RegisterEmployeeCommandResponse>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<RegisterEmployeeCommandResponse>.Failure(Error.InternalServerError("Ocurrió un error tratando de registrar el empleado."));
            }
        }

    }
}
