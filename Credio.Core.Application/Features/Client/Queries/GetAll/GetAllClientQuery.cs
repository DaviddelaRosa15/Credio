using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.Client;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Credio.Core.Application.Features.Client.Queries.GetAll
{
    public class GetAllClientQuery : PaginationRequest, ICachedQuery<List<ClientDTO>>
    {
        [JsonIgnore]
        [SwaggerIgnore]
        public string CachedKey => $"GetAllClientQuery_{PageNumber}_{PageSize}_{OfficerId}";
    }

    public class GetAllClientQueryHandler : IQueryHandler<GetAllClientQuery, List<ClientDTO>>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public GetAllClientQueryHandler(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<ClientDTO>>> Handle(GetAllClientQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var clients = await _clientRepository.GetPagedAsync(query.PageNumber, query.PageSize,
                    new List<Expression<Func<Domain.Entities.Client, object>>>
                    {
                        m => m.DocumentType
                    },
                    !string.IsNullOrWhiteSpace(query.OfficerId)
                        ? (Expression<Func<Domain.Entities.Client, bool>>)(c => c.EmployeeId == query.OfficerId)
                        : null
                 );

                var clientDTOs = _mapper.Map<List<ClientDTO>>(clients.Items);

                return Result<List<ClientDTO>>.Success(clientDTOs);

            }
            catch (Exception ex)
            {
                return Result<List<ClientDTO>>.Failure(Error.InternalServerError("Hubo un error al intentar consultar los clientes"));
            }
        }
    }
}
