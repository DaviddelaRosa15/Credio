using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Constants;
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
    private readonly ISystemSettingsRepository _systemSettingsRepository;
    private readonly ICacheService _cacheService;

    private readonly List<string> _allowedStatuses = ["En Revision", "Pendiente"];

    public ApproveLoanApplicationCommandHandler(
        ILoanApplicationRepository loanApplicationRepository,
        IApplicationStatusRepository applicationStatusRepository,
        ISystemSettingsRepository systemSettingsRepository,
        ICacheService cacheService)
    {
        _loanApplicationRepository = loanApplicationRepository;
        _applicationStatusRepository = applicationStatusRepository;
        _systemSettingsRepository = systemSettingsRepository;
        _cacheService = cacheService;
    }
    
    public async Task<Result> Handle(ApproveLoanApplicationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.LoanApplication? foundApplication =
                await _loanApplicationRepository.GetByIdWithIncludeAsync(x => x.Id == request.LoanApplicationId,
                    [x => x.ApplicationStatus, x => x.PaymentFrequency]);

            if (foundApplication is null) return Result.Failure(Error.NotFound("La aplicacion para el prestamo no fue encontrada"));

            if (!_allowedStatuses.Contains(foundApplication.ApplicationStatus.Name))
            {
                return Result.Failure(Error.BadRequest("La aplicacion para el prestamo ya fue procesada"));
            }

            if (!string.IsNullOrEmpty(foundApplication.RejectionReason)) foundApplication.RejectionReason = null;

            // Traer las configuraciones del sistema para validar la tasa de interes y plazo aprobado
            var systemSettings = await _systemSettingsRepository.GetAllByPropertyAsync(s => s.GroupName.Equals(FinancialRulesSettings.GroupName));

            // Validar la tasa de interes aprobada contra las politicas del sistema
            string validationInterestMessage = ValidateApprovedInteresRate(request.ApprovedInterestRate, systemSettings);
            if (!string.IsNullOrEmpty(validationInterestMessage)) return Result.Failure(Error.BadRequest(validationInterestMessage));

            // Validar el plazo aprobado contra las politicas del sistema
            string validationTermMessage = ValidateApprovedTerm(request.ApprovedTerm, foundApplication.PaymentFrequency, systemSettings);
            if(!string.IsNullOrEmpty(validationTermMessage)) return Result.Failure(Error.BadRequest(validationTermMessage));

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

    private string ValidateApprovedInteresRate(double approvedInterestRate, List<SystemSettings> settings)
    {
        // Buscar las políticas de tasa de interés en la configuración del sistema
        var minSetting = settings.FirstOrDefault(s => s.Key == FinancialRulesSettings.LoanMinInterestRateKey);
        var maxSetting = settings.FirstOrDefault(s => s.Key == FinancialRulesSettings.LoanMaxInterestRateKey);

        if (minSetting == null || maxSetting == null) return "Faltan políticas de tasas en el sistema.";

        decimal.TryParse(minSetting.Value, out decimal minRate);
        decimal.TryParse(maxSetting.Value, out decimal maxRate);

        // Convertir la tasa aprobada a decimal para una comparación precisa
        decimal rateToValidate = (decimal)(approvedInterestRate/100);

        // Validar que la tasa aprobada esté dentro del rango permitido
        if (rateToValidate < minRate || rateToValidate > maxRate)
        {
            return $"La tasa de interés aprobada debe estar entre {minRate * 100}% y {maxRate * 100}%";
        }

        return string.Empty;
    }

    private string ValidateApprovedTerm(int approvedTerm, PaymentFrequency frequency, List<SystemSettings> settings)
    {
        // Buscar las políticas de plazo en la configuración del sistema
        var minSetting = settings.FirstOrDefault(s => s.Key == FinancialRulesSettings.LoanMinTermMonthsKey);
        var maxSetting = settings.FirstOrDefault(s => s.Key == FinancialRulesSettings.LoanMaxTermMonthsKey);

        if (minSetting == null || maxSetting == null) return "Faltan políticas de plazos en el sistema.";

        int minMonths = int.Parse(minSetting.Value);
        int maxMonths = int.Parse(maxSetting.Value);

        // Lógica de normalización: Convertimos el plazo aprobado a meses aproximados para comparar peras con peras
        double termInMonths = frequency.Name switch
        {
            "Semanal" => approvedTerm / 4.345,
            "Quincenal" => approvedTerm / 2.0,
            _ => approvedTerm // Mensual
        };

        // Validar que el plazo aprobado esté dentro del rango permitido
        if (termInMonths < minMonths || termInMonths > maxMonths)
        {
            return $"El plazo solicitado equivale a {termInMonths:N1} meses, lo cual excede el rango permitido ({minMonths}-{maxMonths} meses).";
        }

        return string.Empty;
    }
}