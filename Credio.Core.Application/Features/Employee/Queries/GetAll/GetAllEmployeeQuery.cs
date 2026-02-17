using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.Employee;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Credio.Core.Application.Features.Employee.Queries.GetAll
{
    public class GetAllEmployeeQuery : PaginationRequest, ICachedQuery<List<EmployeeDTO>>
    {
        [JsonIgnore]
        [SwaggerIgnore]
        public string CachedKey => $"GetAllEmployeeQuery_{PageNumber}_{PageSize}";
    }

    public class GetAllEmployeeQueryHandler : IQueryHandler<GetAllEmployeeQuery, List<EmployeeDTO>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public GetAllEmployeeQueryHandler(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<EmployeeDTO>>> Handle(GetAllEmployeeQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var employees = await _employeeRepository.GetPagedAsync(query.PageNumber, query.PageSize,
                    new List<Expression<Func<Domain.Entities.Employee, object>>>
                    {
                        m => m.DocumentType,
                        m => m.Address
                    }
                );

                var employeeDTOs = _mapper.Map<List<EmployeeDTO>>(employees.Items);

                return Result<List<EmployeeDTO>>.Success(employeeDTOs);

            }
            catch (Exception ex)
            {
                return Result<List<EmployeeDTO>>.Failure(Error.InternalServerError("Hubo un error al intentar consultar los empleados"));
            }
        }
    }
}
