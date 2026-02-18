using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.User;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;


namespace Credio.Core.Application.Features.Clients.Queries 
{
    public sealed class GetClientByDocumentNumberQuery: ICachedQuery<ClientDto>
    {
        public GetClientByDocumentNumberQuery(string documentNumber)
        {
            DocumentNumber = documentNumber;    
        }
        
        public string DocumentNumber { get; set; }

        public string CachedKey => $"client-{DocumentNumber}";
    }

    public sealed class GetClientByDocumentQueryHandler : IQueryHandler<GetClientByDocumentNumberQuery, ClientDto>
    {
        private readonly IClientRepository _clientRepository;

        public GetClientByDocumentQueryHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        
        public async Task<Result<ClientDto>> Handle(GetClientByDocumentNumberQuery request, CancellationToken cancellationToken)
        {
            string document = request.DocumentNumber.Trim();
            
            ClientDto? client = await _clientRepository.GetByDocumentNumberAsync(document, cancellationToken);

            if (client is null) return Result<ClientDto>.Failure(Error.NotFound("No se encontro el cliente con el numero de documento dado"));
            
            return Result<ClientDto>.Success(client);
        }
    }
}

