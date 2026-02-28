using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Features.Clients.Commands.UpdateClientCommand;

namespace Credio.Core.Application.Dtos.Requests;

public class UpdateClientRequest
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    
    public int? Age { get; set; }
    
    public string? Email { get; set; }
    
    public string? PhoneNumber { get; set; }

    public AddressDTO? Address { get; set; }
    
    public decimal? HomeLatitude { get; set; }
    
    public decimal? HomeLongitude { get; set; }
    
    public UpdateClientCommand ToCommand(string clientId)
    {
        return new UpdateClientCommand(
            clientId,
            FirstName,
            LastName,
            Age,
            Email,
            PhoneNumber,
            Address,
            HomeLatitude,
            HomeLongitude
        );
    }

}