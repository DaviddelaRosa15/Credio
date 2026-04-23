using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Events;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Core.Application.Features.LoanApplications.Commands.DisburseLoan;

public class DisburseLoanCommand : ICommand<DisburseLoanResponseDTO>
{
    [SwaggerParameter(Description = "Id del prestamo")]
    public string LoanId { get; set; } = string.Empty;

    [SwaggerParameter(Description = "Fecha efectiva del desembolso")]
    public DateOnly EffectiveDate { get; set; }
}

public class DisburseLoanCommandHandler : ICommandHandler<DisburseLoanCommand, DisburseLoanResponseDTO>
{
    private readonly ICacheService _cacheService;
    private readonly ILoanRepository _loanRepository;
    private readonly ILoanStatusRepository _statusRepository;

    public DisburseLoanCommandHandler(
        ICacheService cacheService,
        ILoanRepository loanRepository,
        ILoanStatusRepository statusRepository)
    {
        _cacheService = cacheService;
        _loanRepository = loanRepository;
        _statusRepository = statusRepository;
    }
    
    public async Task<Result<DisburseLoanResponseDTO>> Handle(DisburseLoanCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar si el préstamo existe y no está ya desembolsado
            var loan = await _loanRepository.GetByIdWithIncludeAsync(l => l.Id == request.LoanId,
                [x => x.LoanStatus]);

            // Validar que el préstamo exista y que no esté ya desembolsado
            if (loan is null)
            {
                return Result<DisburseLoanResponseDTO>.Failure(Error.NotFound("El préstamo no existe"));
            }
            else if (loan.LoanStatus.Name != LoanStatuses.Created)
            {
                return Result<DisburseLoanResponseDTO>.Failure(Error.BadRequest("El préstamo ya ha sido desembolsado"));
            }

            //Obtener estado activo
            var statusActive = await _statusRepository.GetByPropertyAsync(s => s.Name == LoanStatuses.Active);

            loan.LoanStatus = statusActive;
            loan.LoanStatusId = statusActive.Id;
            loan.DisbursedDate = request.EffectiveDate;

            loan.AddEvent(new LoanDisbursedEvent(loan.Id, loan.DisbursedDate.Value));

            await _loanRepository.UpdateAsync(loan);

            DisburseLoanResponseDTO response = new()
            {
                LoanId = loan.Id,
                LoanAmount = loan.Amount,
                LoanNumber = loan.LoanNumber,
                LoanStatus = loan.LoanStatus.Name,
                EffectiveDate = loan.DisbursedDate.Value
            };

            _cacheService.RemoveByPrefix("loan-all-");

            return Result<DisburseLoanResponseDTO>.Success(response);
        }
        catch 
        {
            return Result<DisburseLoanResponseDTO>.Failure(Error.InternalServerError("Ocurrió un error al intentar desembolsar el préstamo"));
        }
    }
}