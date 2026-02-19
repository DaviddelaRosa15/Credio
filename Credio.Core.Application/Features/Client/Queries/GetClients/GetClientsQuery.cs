using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.User;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace Credio.Core.Application.Features.Clients.Queries;

public class GetClientsQuery : PaginationRequest, IQuery<PagedResult<ClientDto>>
{
    [SwaggerParameter(Description = "Término de búsqueda (nombre o cédula)", Required = false)]
    public string? SearchTerm { get; set; }

    [SwaggerParameter(Description = "Id del oficial asignado (opcional)", Required = false)]
    public string? OfficerId { get; set; }
}

public class GetClientsQueryHandler : IQueryHandler<GetClientsQuery, PagedResult<ClientDto>>
{
    private readonly IClientRepository _clientRepository;

    public GetClientsQueryHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Result<PagedResult<ClientDto>>> Handle(
        GetClientsQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            PagedResult<ClientDto> result = await _clientRepository.GetClientsPagedAsync(
                pageNumber: query.PageNumber,
                pageSize: query.PageSize,
                searchTerm: query.SearchTerm,
                officerId: query.OfficerId,
                cancellationToken: cancellationToken);

            return Result<PagedResult<ClientDto>>.Success(result);
        }
        catch
        {
            return Result<PagedResult<ClientDto>>.Failure(
                Error.InternalServerError("Ocurrió un error al obtener el listado de clientes."));
        }
    }
}