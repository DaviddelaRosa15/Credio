using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Core.Application.Features.Clients.Commands.DeleteClientCommand;

public record DeleteClientCommand(
    [SwaggerParameter(Description = "Id del cliente")]
    string cliendId): ICommand;

public class DeleteClientCommandHandler : ICommandHandler<DeleteClientCommand>
{
    private readonly IClientRepository _clientRepository;

    public DeleteClientCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    public async Task<Result> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var foundClient = await _clientRepository.GetByIdAsync(request.cliendId);

            if (foundClient is null) return Result.Failure(Error.NotFound("No se encontro el empleado con el id proporcionado"));
        
            await _clientRepository.DeleteAsync(foundClient);
        
            return Result.Success();
        }
        catch 
        {
            return Result.Failure(Error.InternalServerError("Ocurrio un error al eliminar el cliente"));
        }
    }
}