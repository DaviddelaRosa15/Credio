using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Client;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;


namespace Credio.Core.Application.Features.Clients.Queries
{
    public sealed class GetClientByIdQuery : ICachedQuery<ClientDetailDto>
    {
        public GetClientByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

        public Guid Id { get; set; }

        public string CachedKey => $"client-{Id}";
    }

    public sealed class GetClientByIdQueryHandler : IQueryHandler<GetClientByIdQuery, ClientDetailDto>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public GetClientByIdQueryHandler(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public async Task<Result<ClientDetailDto>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                return Result<ClientDetailDto>.Failure(Error.BadRequest("El campo id no puede estar vacio"));
            }

            Guid Id = request.Id;
    
            var foundClient = await _clientRepository
                .GetByPropertyWithIncludeAsync(
                    x => x.Id == Id.ToString(),
                    [
                        x => x.Address,
                        x => x.DocumentType,
                        x => x.DocumentNumber,
                        x => x.HomeLatitude,
                        x => x.HomeLongitude
                    ]
                );

            if (foundClient is null) return Result<ClientDetailDto>.Failure(Error.NotFound("No se encontro el cliente con el id dado"));

            var ClientDetailDto = _mapper.Map<ClientDetailDto>(foundClient);

            return Result<ClientDetailDto>.Success(ClientDetailDto);
        }
    }
}
