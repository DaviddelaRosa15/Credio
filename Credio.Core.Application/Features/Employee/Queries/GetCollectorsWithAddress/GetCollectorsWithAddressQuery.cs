using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.Employee;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Credio.Core.Application.Features.Employee.Queries.GetCollectorsWithAddress
{
    public class GetCollectorsWithAddressQuery : PaginationRequest, ICachedQuery<List<EmployeeCollectorDTO>>
    {
        [JsonIgnore]
        [SwaggerIgnore]
        public string CachedKey => $"GetCollectorsWithAddressQuery{PageNumber}_{PageSize}";
    }

    public class GetCollectorsWithAddressQueryHandler : IQueryHandler<GetCollectorsWithAddressQuery, List<EmployeeCollectorDTO>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public GetCollectorsWithAddressQueryHandler(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<EmployeeCollectorDTO>>> Handle(GetCollectorsWithAddressQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var employees = await _employeeRepository.GetPagedAsync(query.PageNumber, query.PageSize,
                    new List<Expression<Func<Domain.Entities.Employee, object>>>
                    {
                        m => m.Address
                    },
                    e => e.IsCollector == true
                );

                var employeeDTOs = _mapper.Map<List<EmployeeCollectorDTO>>(employees.Items);


                return Result<List<EmployeeCollectorDTO>>.Success(employeeDTOs);

            }
            catch (Exception ex)
            {
                return Result<List<EmployeeCollectorDTO>>.Failure(Error.InternalServerError("Hubo un error al intentar consultar los cobradores"));
            }
        }
    }
}
