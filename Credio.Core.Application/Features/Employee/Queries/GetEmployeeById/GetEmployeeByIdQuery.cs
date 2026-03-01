using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Employee;
using Credio.Core.Application.Features.Employee.Queries.GetEmployeeByCode;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Credio.Core.Application.Features.Employee.Queries.GetEmployeeById
{
    public sealed class GetEmployeeByIdQuery : ICachedQuery<EmployeeDetailDTO>
    {
        public GetEmployeeByIdQuery(string Id)
        {
            this.Id = Id;
        }

        public string Id { get; set; }

        public string CachedKey => $"Employee-{Id}";
    }

    public sealed class GetEmployeeByIdQueryHandler : IQueryHandler<GetEmployeeByIdQuery, EmployeeDetailDTO>
    {
        private readonly IEmployeeRepository _emplopyeeRepository;
        private readonly IMapper _mapper;

        public GetEmployeeByIdQueryHandler(IEmployeeRepository emplopyeeRepository, IMapper mapper)
        {
            _emplopyeeRepository = emplopyeeRepository;
            _mapper = mapper;
        }

        public async Task<Result<EmployeeDetailDTO>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Id))
                {
                    return Result<EmployeeDetailDTO>.Failure(
                        Error.BadRequest("El campo id de empleado no puede estar vacio")
                    );
                }

                var foundEmployee = await _emplopyeeRepository.GetByIdWithIncludeAsync(x => x.Id == request.Id,
                    [
                        x => x.Address,
                        x => x.Clients
                    ]
                );


                if (foundEmployee is null) return Result<EmployeeDetailDTO>.Failure(Error.NotFound("No se encontro el empleado con el id dado"));

                var EmployeeDTO = _mapper.Map<EmployeeDetailDTO>(foundEmployee);

                return Result<EmployeeDetailDTO>.Success(EmployeeDTO);
            }
            catch (Exception ex)
            {
                return Result<EmployeeDetailDTO>.Failure(Error.InternalServerError($"Ocurrio un error inesperado consultando el empleado"));
            }
        }
    }
}
