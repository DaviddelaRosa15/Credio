using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.User;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;


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
            if (string.IsNullOrEmpty(request.DocumentNumber) || string.IsNullOrWhiteSpace(request.DocumentNumber))
            {
                return Result<ClientDto>.Failure(Error.BadRequest("The document number can't be null or empty"));
            }
            
            string document = request.DocumentNumber.Trim();
            
            Client? foundClient = await _clientRepository.GetByPropertyWithIncludeAsync(x => x.DocumentNumber == document, [x=> x.Address]);

            if (foundClient is null) return Result<ClientDto>.Failure(Error.NotFound("No se encontro el cliente con el numero de documento dado"));
            
            return Result<ClientDto>.Success(new ClientDto
            {
                Id =  foundClient.Id,
                FullName = $"{foundClient.FirstName} {foundClient.LastName}",
                DocumentNumber =  foundClient.DocumentNumber,
                City = foundClient.Address.City,
                Phone = foundClient.Phone,
                State = foundClient.Address.Region
            });
        }
    }
}

