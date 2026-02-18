using Credio.Core.Application.Dtos.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Core.Application.Features.Clients.Commands.CreateClientCommand;

public class CreateClientCommandResponse
{
    [SwaggerSchema(Description = "Identificador del cliente")]
    public string Id { get; set; }

    [SwaggerSchema(Description = "Nombre")]
    public string FirstName { get; set; }

    [SwaggerSchema(Description = "Apellido")]
    public string LastName { get; set; }
    
    [SwaggerSchema(Description = "Edad")]
    public int Age { get; set; }

    [SwaggerSchema(Description = "Nombre de usuario")]
    public string UserName { get; set; }

    [SwaggerSchema(Description = "Tipo de documento")]
    public string DocumentType { get; set; }

    [SwaggerSchema(Description = "Número de documento")]
    public string DocumentNumber { get; set; }

    [SwaggerSchema(Description = "Teléfono")]
    public string PhoneNumber { get; set; }

    [SwaggerSchema(Description = "Correo")]
    public string? Email { get; set; }

    [SwaggerSchema(Description = "Dirección")]
    public AddressDTO Address { get; set; }

    [SwaggerSchema(Description = "Foto de perfil")]
    public string? UrlImage { get; set; }
    
    [SwaggerSchema(Description = "Latitud")]
    public decimal HomeLatitude { get; set; }
    
    [SwaggerSchema(Description = "Longitud")]
    public decimal HomeLongitude { get; set; }
}