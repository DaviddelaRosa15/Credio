using AutoMapper;
using Avalanche.Core.Application.Helpers;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using RegisterRequest = Credio.Core.Application.Dtos.Account.RegisterRequest;

namespace Credio.Core.Application.Features.Clients.Commands.CreateClientCommand;

public class CreateClientCommand : ICommand
{
    [SwaggerParameter(Description = "Primer Nombre")]
    public string FirstName { get; set; } 
    
    [SwaggerParameter(Description = "Apellido")]
    public string LastName { get; set; }

    [SwaggerParameter(Description = "Edad")]
    public int Age { get; set; }
    
    [SwaggerParameter(Description = "Email")]
    public string Email { get; set; }
    
    [SwaggerParameter(Description = "Tipo de documento")]
    public string DocumentType { get; set; }
    
    [SwaggerParameter(Description = "Teléfono")]
    public string Phone { get; set; }
    
    [SwaggerParameter(Description = "Numero del documento")]
    public string DocumentNumber { get; set; }

    [SwaggerParameter(Description = "Dirección")]
    public AddressDTO AddressDto { get; set; }
    
    [SwaggerParameter(Description = "Id del empleado")]
    public string EmployeeId { get; set; }
    
    [SwaggerParameter(Description = "Latitud")]
    public decimal HomeLatitude { get; set; }
    
    [SwaggerParameter(Description = "Longitud")]
    public decimal HomeLongitude { get; set; }
    
    [SwaggerParameter(Description = "Foto de perfil")]
    public IFormFile? Image { get; set; }
}

public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand>
{
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;

    public CreateClientCommandHandler(
        IDocumentTypeRepository documentTypeRepository,
        IClientRepository clientRepository,
        IEmployeeRepository employeeRepository,
        IMapper mapper, 
        IAccountService accountService)
    {
        _documentTypeRepository = documentTypeRepository;
        _clientRepository = clientRepository;
        _employeeRepository = employeeRepository;
        _mapper = mapper;
        _accountService = accountService;
    }
    
    public async Task<Result> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        DocumentType? documentType = await _documentTypeRepository.GetByPropertyAsync(x => x.Name == request.DocumentType);

        if (documentType is null) return Result.Failure(Error.BadRequest("El tipo de documento proporcionado no existe."));
        
        var currentEmployee = await _employeeRepository.GetByIdAsync(request.EmployeeId);

        if (currentEmployee is null) return Result.Failure(Error.BadRequest("El empleado no fue encontrado"));

        if (await _clientRepository.IsDocumentNumberRegister(request.DocumentNumber, cancellationToken))
        {
            return Result.Failure(Error.Conflict("El numero de documento ya existe."));
        }
        
        RegisterRequest clientRequest = new RegisterRequest
        {
            FirstName =  request.FirstName,
            LastName =  request.LastName,
            UserName = $"Credio@{request.FirstName}{request.LastName}",
            Email = request.Email,
            Address = $"{request.AddressDto.City} {request.AddressDto.AddressLine1} {request.AddressDto.AddressLine2}",
            PhoneNumber = request.Phone,
            Password = "Credio@" + Guid.NewGuid().ToString().Substring(0, 4),
            UrlImage = request.Image is not null ? ImageUpload.UploadImage(request.Image, StorageConstants.Clients) : ""
        };
        
        RegisterResponse response = await _accountService.RegisterClientAsync(clientRequest);

        if (response.Status == "Fallido")
        {
            ImageUpload.DeleteFile(clientRequest.UrlImage);
            return Result.Failure(Error.BadRequest(response.Details[0].Message));
        }

        Client client = new Client
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Age = request.Age,
            DocumentNumber = request.DocumentNumber,
            DocumentTypeId = documentType.Id,
            EmployeeId = currentEmployee.Id,
            HomeLatitude = request.HomeLatitude,
            HomeLongitude = request.HomeLongitude,
            Address = _mapper.Map<Address>(request.AddressDto),
            UserId = response.Id,
            Email = request.Email,
            Phone = request.Phone,
        };
        
         await _clientRepository.AddAsync(client);

        return Result.Success();
    }
}