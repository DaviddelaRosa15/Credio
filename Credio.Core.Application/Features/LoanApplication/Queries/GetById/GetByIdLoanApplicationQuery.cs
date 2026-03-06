using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.LoanApplication;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;

namespace Credio.Core.Application.Features.LoanApplications.Queries.GetById
{
    public sealed class GetByIdLoanApplicationQuery : ICachedQuery<LoanApplicationDto>
    {
        public GetByIdLoanApplicationQuery(string Id) 
        {
            this.Id = Id;
        }

        public string Id { get; private set; }

        public string CachedKey => $"loan-application-{Id}";
    }

    public sealed class GetByIdLoanApplicationQueryHandler : IQueryHandler<GetByIdLoanApplicationQuery, LoanApplicationDto>
    {
        private readonly ILoanApplicationRepository _loanRepository;
        private readonly IMapper _mapper;

        public GetByIdLoanApplicationQueryHandler(ILoanApplicationRepository loanRepository, IMapper mapper)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
        }

        public async Task<Result<LoanApplicationDto>> Handle(GetByIdLoanApplicationQuery request, CancellationToken cancellationToken)
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


                if (foundLoan is null) return Result<LoanApplicationDto>.Failure(Error.NotFound("No se encontro la solicitud con el id dado"));

                var loansDTO = _mapper.Map<LoanApplicationDto>(foundLoan);

                return Result<LoanApplicationDto>.Success(loansDTO);
            }
            catch (Exception ex)
            {
                return Result<LoanApplicationDto>.Failure(Error.InternalServerError($"Ocurrio un error inesperado consultando la solicitud"));
            }
        }
    }
}
