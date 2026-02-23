using AutoMapper;
using Avalanche.Core.Application.Helpers;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.Email;
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
        private string _generatedPassword;

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


                //Validar que no exista otro empleado con el mismo número de documento
                var existingDocument = await _employeeRepository.GetByPropertyAsync(e => e.DocumentNumber == command.DocumentNumber);
                if (existingDocument != null)
                {
                    return Result<RegisterEmployeeCommandResponse>.Failure(Error.Conflict("Ya existe un empleado registrado con el número de documento proporcionado."));
                }

                //Validar que no exista otro empleado con el mismo correo electrónico
                var existingEmail = await _employeeRepository.GetByPropertyAsync(e => e.Email == command.Email);
                if (existingEmail != null)
                {
                    return Result<RegisterEmployeeCommandResponse>.Failure(Error.Conflict("Ya existe un empleado registrado con el correo electrónico proporcionado."));
                }

                //Obtener el último código de empleado registrado para generar el nuevo código
                var lastEmployeeCode = await _employeeRepository.GetLastEmployeeCodeAsync();
                employee.EmployeeCode = $"U{lastEmployeeCode + 1}";

                //Registro de usuario en Identity
                var response = await RegisterUser(command, employee.EmployeeCode);
                employee.UserId = response.Id;

                //Registro de dirección
                var employeeAddress = _mapper.Map<Address>(command.Address);
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
                result.UserName = employee.EmployeeCode;
                result.Address = command.Address;
                result.Role = command.Role;
                result.UrlImage = response.UrlImage;

                await SendEmployeeWelcomeEmail(result);

                return Result<RegisterEmployeeCommandResponse>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<RegisterEmployeeCommandResponse>.Failure(Error.InternalServerError("Ocurrió un error tratando de registrar el empleado."));
            }
        }

        private async Task<RegisterResponse> RegisterUser(RegisterEmployeeCommand command, string employeeCode)
        {
            var request = _mapper.Map<RegisterRequest>(command);

            //Generar nombre de usuario y contraseña temporal
            request.UserName = employeeCode;
            request.Password = "Credio@" + Guid.NewGuid().ToString().Substring(0, 4);
            _generatedPassword = request.Password;
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
            switch (command.Role.ToLower())
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
                throw new Exception(response.Details[0].Message);
            }

            return response;
        }

        private async Task SendEmployeeWelcomeEmail(RegisterEmployeeCommandResponse result)
        {
            try
            {
                // Traducir el rol al español para el correo
                string roleInSpanish = result.Role.ToLower() switch
                {
                    "administrator" => "Administrador",
                    "collector" => "Cobrador",
                    "officer" => "Oficial",
                    _ => result.Role
                };

                EmployeeWelcomeEmail dto = new()
                {
                    FullName = $"{result.FirstName} {result.LastName}",
                    UserName = result.UserName,
                    TemporaryPassword = _generatedPassword,
                    Role = roleInSpanish
                };

                await _emailService.SendAsync(new EmailRequest()
                {
                    To = result.Email,
                    Body = _emailHelper.MakeEmailForEmployee(dto),
                    Subject = $"¡Bienvenido/a como {roleInSpanish} en Credio!"
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error enviando el correo al administrador");
            }
        } 
    }
}
