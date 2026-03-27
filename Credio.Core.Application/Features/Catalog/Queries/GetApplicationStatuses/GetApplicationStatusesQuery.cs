using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Catalog;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Credio.Core.Application.Features.Catalog.Queries.GetApplicationStatuses
{
    public class GetApplicationStatusesQuery : PaginationRequest, ICachedQuery<List<ApplicationStatusDTO>>
    {
        [JsonIgnore]
        [SwaggerIgnore]
        public string CachedKey => $"GetApplicationStatusesQuery_{PageNumber}_{PageSize}";
    }

    public class GetApplicationStatusesQueryHandler : IQueryHandler<GetApplicationStatusesQuery, List<ApplicationStatusDTO>>
    {
        private readonly IApplicationStatusRepository _applicationStatusRepository;
        private readonly IMapper _mapper;

        public GetApplicationStatusesQueryHandler(IApplicationStatusRepository applicationStatusRepository, IMapper mapper)
        {
            _applicationStatusRepository = applicationStatusRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<ApplicationStatusDTO>>> Handle(GetApplicationStatusesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var Application = await _applicationStatusRepository.GetPagedAsync(query.PageNumber, query.PageSize,
                    new List<Expression<Func<Domain.Entities.ApplicationStatus, object>>>
                    {
                        
                    }
                );

                var applicationDTOs = _mapper.Map<List<ApplicationStatusDTO>>(Application.Items);

                return Result<List<ApplicationStatusDTO>>.Success(applicationDTOs);

            }
            catch (Exception ex)
            {
                return Result<List<ApplicationStatusDTO>>.Failure(Error.InternalServerError("Hubo un error al intentar consultar los estados de las solicitudes de crédito"));
            }
        }
    }
}
