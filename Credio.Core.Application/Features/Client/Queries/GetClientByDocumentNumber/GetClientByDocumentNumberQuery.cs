using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Client;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;


namespace Credio.Core.Application.Features.Clients.Queries 
{
    public sealed class GetClientByDocumentNumberQuery: ICachedQuery<ClientDTO>
    {
        public GetClientByDocumentNumberQuery(string documentNumber)
        {
            DocumentNumber = documentNumber;    
        }
        
        public string DocumentNumber { get; set; }

        public string CachedKey => $"client-{DocumentNumber}";
    }

    public sealed class GetClientByDocumentQueryHandler : IQueryHandler<GetClientByDocumentNumberQuery, ClientDTO>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public GetClientByDocumentQueryHandler(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }
        
        public async Task<Result<ClientDTO>> Handle(GetClientByDocumentNumberQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.DocumentNumber) || string.IsNullOrWhiteSpace(request.DocumentNumber))
            {
                return Result<ClientDTO>.Failure(Error.BadRequest("El numero de documento no puede estar vacio"));
            }

            string document = request.DocumentNumber.Trim();

            var foundClient = await _clientRepository.GetByPropertyWithIncludeAsync(x => x.DocumentNumber == document, [x => x.Address, x => x.DocumentType]);

            if (foundClient is null) return Result<ClientDTO>.Failure(Error.NotFound("No se encontro el cliente con el numero de documento dado"));

            var clientDTO = _mapper.Map<ClientDTO>(foundClient);

            return Result<ClientDTO>.Success(clientDTO);
        }
    }
}