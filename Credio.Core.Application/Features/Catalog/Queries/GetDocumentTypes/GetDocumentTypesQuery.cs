using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Catalog;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Credio.Core.Application.Features.Catalog.Queries.GetDocumentTypes
{
    public class GetDocumentTypesQuery : PaginationRequest, ICachedQuery<List<DocumentTypeDTO>>
    {
        [JsonIgnore]
        [SwaggerIgnore]
        public string CachedKey => $"GetDocumentTypesQuery_{PageNumber}_{PageSize}";
    }

    public class GetDocumentTypesQueryHandler : IQueryHandler<GetDocumentTypesQuery, List<DocumentTypeDTO>>
    {
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IMapper _mapper;

        public GetDocumentTypesQueryHandler(IDocumentTypeRepository documentTypeRepository, IMapper mapper)
        {
            _documentTypeRepository = documentTypeRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<DocumentTypeDTO>>> Handle(GetDocumentTypesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var Document = await _documentTypeRepository.GetPagedAsync(query.PageNumber, query.PageSize,
                    new List<Expression<Func<Domain.Entities.DocumentType, object>>>
                    {
   
                    }
                );

                var documentTypeDTOs = _mapper.Map<List<DocumentTypeDTO>>(Document.Items);

                return Result<List<DocumentTypeDTO>>.Success(documentTypeDTOs);

            }
            catch (Exception ex)
            {
                return Result<List<DocumentTypeDTO>>.Failure(Error.InternalServerError("Hubo un error al intentar consultar los tipos de documento"));
            }
        }
    }
}
