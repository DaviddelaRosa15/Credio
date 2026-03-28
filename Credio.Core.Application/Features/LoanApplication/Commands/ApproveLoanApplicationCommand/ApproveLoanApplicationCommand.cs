using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Core.Application.Features.LoanApplications.Commands.ApproveLoanApplicationCommand;

public class ApproveLoanApplicationCommand : ICommand
{
    [SwaggerParameter(Description = "Id de la aplicacion del prestamo")]
    public string LoanApplicationId { get; set; } = string.Empty;
    
    [SwaggerParameter(Description = "Monto aprobado")]
    public double ApprovedAmount { get; set; }
    
    [SwaggerParameter(Description = "Termino aprobado")]
    public int ApprovedTerm { get; set; }
    
    [SwaggerParameter(Description = "Tasa aprobada")]
    public double ApprovedInterestRate { get; set; }

    public void Apply(Domain.Entities.LoanApplication loanApplication)
    {
        loanApplication.ApprovedAmount =  ApprovedAmount;
        loanApplication.ApprovedTerm = ApprovedTerm;
        loanApplication.ApprovedInterestRate = ApprovedInterestRate;
    }
}

public class ApproveLoanApplicationCommandHandler : ICommandHandler<ApproveLoanApplicationCommand>
{
    private readonly ILoanApplicationRepository _loanApplicationRepository;
    private readonly IApplicationStatusRepository _applicationStatusRepository;
    private readonly ICacheService _cacheService;

    private readonly List<string> _allowedStatuses = ["En Revision", "Pendiente"];

    public ApproveLoanApplicationCommandHandler(
        ILoanApplicationRepository loanApplicationRepository,
        IApplicationStatusRepository applicationStatusRepository,
        ICacheService cacheService)
    {
        _loanApplicationRepository = loanApplicationRepository;
        _applicationStatusRepository = applicationStatusRepository;
        _cacheService = cacheService;
    }
    
    public async Task<Result> Handle(ApproveLoanApplicationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.LoanApplication? foundApplication =
                await _loanApplicationRepository.GetByIdWithIncludeAsync(x => x.Id == request.LoanApplicationId,
                    [x => x.ApplicationStatus]);

            if (foundApplication is null) return Result.Failure(Error.NotFound("La aplicacion para el prestamo no fue encontrada"));

            if (!_allowedStatuses.Contains(foundApplication.ApplicationStatus.Name))
            {
                return Result.Failure(Error.BadRequest("La aplicacion para el prestamo ya fue procesada"));
            }

            if (!string.IsNullOrEmpty(foundApplication.RejectionReason)) foundApplication.RejectionReason = null;
        
            request.Apply(foundApplication);

            ApplicationStatus? approvedStatus = await _applicationStatusRepository.GetByPropertyAsync(x => x.Name == "Aprobada");

            if (approvedStatus is null) return Result.Failure(Error.NotFound("No se pudo encontrar el estado de aplicacion de aprobada"));

            foundApplication.ApplicationStatus = approvedStatus;
        
            await _loanApplicationRepository.UpdateAsync(foundApplication);
            
            _cacheService.Remove($"loan-application-{foundApplication.Id}");
            
            _cacheService.RemoveByPrefix("GetAllLoanApplicationsQuery_");

            return Result.Success();
        }
        catch
        {
            return Result.Failure(Error.InternalServerError("Ocurrio un error al aprobar la solicitud de prestamo"));
        }
    }
}