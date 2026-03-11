using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Features.LoanApplications.Commands.RejectLoanApplicationCommand;

public class RejectLoanApplicationCommand : ICommand
{
    public string LoanApplicationId { get; set; } = string.Empty;

    public string RejectReason { get; set; } = string.Empty;

    public void Apply(LoanApplication loanApplication)
    {
        loanApplication.RejectionReason = RejectReason;
    }
}

public class RejectLoanApplicationCommandHandler : ICommandHandler<RejectLoanApplicationCommand>
{
    private readonly ILoanApplicationRepository _loanApplicationRepository;
    private readonly IApplicationStatusRepository _applicationStatusRepository;

    private readonly List<string> _allowedStatuses = ["En Revision", "Pendiente"];

    public RejectLoanApplicationCommandHandler(
        ILoanApplicationRepository loanApplicationRepository,
        IApplicationStatusRepository applicationStatusRepository)
    {
        _loanApplicationRepository = loanApplicationRepository;
        _applicationStatusRepository = applicationStatusRepository;
    }
    
    public async Task<Result> Handle(RejectLoanApplicationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            LoanApplication? foundApplication = await _loanApplicationRepository
                .GetByIdWithIncludeAsync(x => x.Id == request.LoanApplicationId, [x => x.ApplicationStatus]);

            if (foundApplication is null) return Result.Failure(Error.NotFound("La aplicacion para el prestamo no fue encontrada"));

            if (!_allowedStatuses.Contains(foundApplication.ApplicationStatus.Name))
            {
                return Result.Failure(Error.BadRequest("La aplicacion para el prestamo ya fue procesada"));
            }
        
            request.Apply(foundApplication);

            ApplicationStatus? rejectStatus = await _applicationStatusRepository.GetByPropertyAsync(x => x.Name == "Rechazada");

            if (rejectStatus is null) return Result.Failure(Error.NotFound("No se pudo encontrar el estado de solicitud de rechazada"));
        
            foundApplication.ApplicationStatus = rejectStatus;
        
            await _loanApplicationRepository.UpdateAsync(foundApplication); 

            return Result.Success();
        }
        catch
        {
            return Result.Failure(Error.InternalServerError("Ocurrio un error al rechazar una solicitud de prestamo"));
        }
    }
}