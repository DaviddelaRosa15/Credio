using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Client;
using Credio.Core.Application.Dtos.LoanApplication;
using Credio.Core.Application.Features.Clients.Queries;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Credio.Core.Application.Features.LoanApplication
{
    public sealed class GetLoanApplicationByIdQuery : ICachedQuery<LoanApplicationDto>
    {
        public GetLoanApplicationByIdQuery(string Id) 
        {
            this.Id = Id;
        }

        public string Id { get; private set; }

        public string CachedKey => $"loans-{Id}";
    }

    public sealed class GetLoanApplicationByIdQueryHandler : IQueryHandler<GetLoanApplicationByIdQuery, LoanApplicationDto>
    {
        private readonly ILoanApplicationRepository _loanRepository;
        private readonly IMapper _mapper;

        public GetLoanApplicationByIdQueryHandler(ILoanApplicationRepository loanRepository, IMapper mapper)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
        }

        public async Task<Result<LoanApplicationDto>> Handle(GetLoanApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Id))
                {
                    return Result<LoanApplicationDto>.Failure(
                        Error.BadRequest("El campo id no puede estar vacio")
                    );
                }

                var foundLoan = await _loanRepository.GetByIdWithIncludeAsync(x => x.Id == request.Id,
                    [
                        x => x.ApplicationStatus,
                        x => x.Client
                    ]
                );


                if (foundLoan is null) return Result<LoanApplicationDto>.Failure(Error.NotFound("No se encontro el prestamo con el id dado"));

                var loansDTO = _mapper.Map<LoanApplicationDto>(foundLoan);

                return Result<LoanApplicationDto>.Success(loansDTO);
            }
            catch (Exception ex)
            {
                return Result<LoanApplicationDto>.Failure(Error.InternalServerError($"Ocurrio un error inesperado consultando el prestamo"));
            }
        }
    }
}
