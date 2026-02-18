using MediatR;
using Credio.Core.Application.Dtos.User;
using Credio.Core.Application.Features.Clients.Queries;
using Credio.Core.Application.Interfaces.Clients;


namespace Credio.Core.Application.Features.Clients.Queries 
{
    public sealed class GetClientByDocumentNumberQueryHandler
    : IRequestHandler<GetClientByDocumentNumberQuery, ClientListDto>
    {
        private readonly IClientQueryRepository _clientQueryRepository;

        public GetClientByDocumentNumberQueryHandler(
            IClientQueryRepository clientQueryRepository)
        {
            _clientQueryRepository = clientQueryRepository;
        }

        public async Task<ClientListDto> Handle(
            GetClientByDocumentNumberQuery request,
            CancellationToken cancellationToken)
        {
            var document = request.DocumentNumber?.Trim();

            var client = await _clientQueryRepository
                .GetByDocumentNumberAsync(document, cancellationToken);

            if (client is null)
                throw new KeyNotFoundException("Client not found");

            return client;
        }
    }
}


