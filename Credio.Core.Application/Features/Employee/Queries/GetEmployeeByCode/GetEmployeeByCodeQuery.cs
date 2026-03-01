using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Client;
using Credio.Core.Application.Dtos.Employee;
using Credio.Core.Application.Features.Clients.Queries;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Credio.Core.Application.Features.Employee.Queries.GetEmployeeByCode
{
    public sealed class GetEmployeeByCodeQuery : ICachedQuery<EmployeeDetailDTO>
    {
        public GetEmployeeByCodeQuery(string EmployeeCode)
        {
            this.EmployeeCode = EmployeeCode;
        }

        public string EmployeeCode { get; set; }

        public string CachedKey => $"Employee-{EmployeeCode}";
    }

    public sealed class GetEmployeeByCodeQueryHandler : IQueryHandler<GetEmployeeByCodeQuery, EmployeeDetailDTO>
    {
        private readonly IEmployeeRepository _emplopyeeRepository;
        private readonly IMapper _mapper;

        public GetEmployeeByCodeQueryHandler(IEmployeeRepository emplopyeeRepository, IMapper mapper)
        {
            _emplopyeeRepository = emplopyeeRepository;
            _mapper = mapper;
        }

        public async Task<Result<EmployeeDetailDTO>> Handle(GetEmployeeByCodeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.EmployeeCode))
                {
                    return Result<EmployeeDetailDTO>.Failure(
                        Error.BadRequest("El campo codigo de empleado no puede estar vacio")
                    );
                }

                var foundEmployee = await _emplopyeeRepository.GetByIdWithIncludeAsync(x => x.EmployeeCode == request.EmployeeCode,
                    [
                        x => x.Address,
                        x => x.Clients
                    ]
                );


                if (foundEmployee is null) return Result<EmployeeDetailDTO>.Failure(Error.NotFound("No se encontro el empleado con el codigo dado"));

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
