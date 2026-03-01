using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Client;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;


namespace Credio.Core.Application.Features.Clients.Queries
{
    public sealed class GetClientByIdQuery : ICachedQuery<ClientDetailDTO>
    {
        public GetClientByIdQuery(string Id)
        {
            this.Id = Id;
        }

        public string Id { get; set; }

        public string CachedKey => $"client-{Id}";
    }

    public sealed class GetClientByIdQueryHandler : IQueryHandler<GetClientByIdQuery, ClientDetailDTO>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public GetClientByIdQueryHandler(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public async Task<Result<ClientDetailDTO>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Id))
                {
                    return Result<ClientDetailDTO>.Failure(
                        Error.BadRequest("El campo id no puede estar vacio")
                    );
                }

                var foundClient = await _clientRepository.GetByIdWithIncludeAsync(x => x.Id == request.Id,
                    [
                        x => x.Address,
                        x => x.DocumentType
                    ]
                );


                if (foundClient is null) return Result<ClientDetailDTO>.Failure(Error.NotFound("No se encontro el cliente con el id dado"));

                var clientDTO = _mapper.Map<ClientDetailDTO>(foundClient);

                return Result<ClientDetailDTO>.Success(clientDTO);
            }
            catch (Exception ex)
            {
                return Result<ClientDetailDTO>.Failure(Error.InternalServerError($"Ocurrio un error inesperado consultando el cliente"));
            }
        }
    }
}