using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Core.Application.Features.Clients.Commands.UpdateClientCommand;

public record UpdateClientCommand(
    [SwaggerParameter(Description = "Id del cliente")]
    string clientId,
    [SwaggerParameter(Description = "Primer Nombre", Required = false)]
    string? firstName,
    [SwaggerParameter(Description = "Apellido", Required = false)]
    string? lastName,
    [SwaggerParameter(Description = "Edad", Required = false)]
    int? age,
    [SwaggerParameter(Description = "Correo", Required = false)]
    string? email,
    [SwaggerParameter(Description = "Numero telefonico", Required = false)]
    string? phone,
    [SwaggerParameter(Description = "Direccion", Required = false)]
    AddressDTO? addressDto,
    [SwaggerParameter(Description = "Latitud", Required = false)]
    decimal? homeLatitude,
    [SwaggerParameter(Description = "Longitud", Required = false)]
    decimal? homeLongitude) : ICommand
{
    public void Apply(Client client)
    {
        client.FirstName = firstName ?? client.FirstName;
        client.LastName  = lastName  ?? client.LastName;
        client.Age       = age       ?? client.Age;
        client.Email     = email     ?? client.Email;
        client.Phone     = phone     ?? client.Phone;
        client.HomeLatitude = homeLatitude ?? client.HomeLatitude;
        client.HomeLongitude = homeLongitude ?? client.HomeLongitude;

        if (addressDto is not null)
        {
            client.Address.StreetNumber = addressDto.StreetNumber ?? client.Address.StreetNumber;
            client.Address.AddressLine1 = addressDto.AddressLine1 ?? client.Address.AddressLine1;
            client.Address.AddressLine2 = addressDto.AddressLine2 ?? client.Address.AddressLine2;
            client.Address.City         = addressDto.City ?? client.Address.City;
            client.Address.Region       = addressDto.Region ?? client.Address.Region;
            client.Address.PostalCode   = addressDto.PostalCode ?? client.Address.PostalCode;
        }
    }
}

public class UpdateClientCommandHandler : ICommandHandler<UpdateClientCommand>
{
    private readonly IClientRepository _clientRepository;

    public UpdateClientCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    public async Task<Result> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        Client? foundClient = await _clientRepository
            .GetByIdWithIncludeAsync(x => x.Id == request.clientId, [(x => x.Address)]);

        if (foundClient is null)
        {
            return Result.Failure(Error.BadRequest("El cliente no fue encontrado"));
        }
    
        request.Apply(foundClient);
        
        await _clientRepository.UpdateAsync(foundClient);

        return Result.Success();
    }
}