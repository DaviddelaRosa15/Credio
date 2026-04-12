using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.CoreConfiguration;
using Credio.Core.Application.Dtos.Email;
using Credio.Core.Application.Interfaces.Helpers;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;

namespace Credio.Core.Application.Services
{
    public class EodAlertService : IEodAlertService
    {
        private readonly IEmailHelper _emailHelper;
        private readonly IEmailService _emailService;
        private readonly IEndOfDayExecutionLogRepository _endOfDayExecutionLogRepository;
        private readonly ISupportEmailProviderService _supportEmailProviderService;

        public EodAlertService(
            IEmailHelper emailHelper,
            IEmailService emailService,
            IEndOfDayExecutionLogRepository endOfDayExecutionLogRepository,
            ISupportEmailProviderService supportEmailProviderService)
        {
            _emailHelper = emailHelper;
            _emailService = emailService;
            _endOfDayExecutionLogRepository = endOfDayExecutionLogRepository;
            _supportEmailProviderService = supportEmailProviderService;
        }

        public async Task SendEodAlertAsync(string message, string? exception)
        {
            // Crear DTO para notificación
            EodAlertNotificationDTO alert = new();

            // Obtener día actual
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            // Obtener correo técnico de configuración
            string technicalEmail = await _supportEmailProviderService.GetSupportEmailAsync();

            try
            {
                // Obtener log de ejecución del día
                var log = await _endOfDayExecutionLogRepository.GetByPropertyWithIncludeAsync(l => l.ExecutionDate == today, [log => log.QueueItems]);

                if (log is null)
                {
                    alert.ExecutionDate = today;
                    alert.ErrorSummary = "No se llegó a ejecutar el proceso de Fin de Día o no se pudo registrar el log de ejecución.";
                    alert.FailedCount = 0;
                    alert.PendingCount = 0;
                    alert.ProcessedCount = 0;
                    alert.ProcessName = "Proceso de Fin de Día";
                    alert.TechnicalDetails = exception ?? "No se obtuvo información adicional.";
                }
                else
                {
                    alert.ExecutionDate = log.ExecutionDate;
                    alert.ErrorSummary = message;
                    alert.FailedCount = log.QueueItems.Count(q => q.Status == EndOfDayQueueStatuses.Failed);
                    alert.PendingCount = log.QueueItems.Count(q => q.Status == EndOfDayQueueStatuses.Pending);
                    alert.ProcessedCount = log.ProcessedLoans;
                    alert.ProcessName = "Proceso de Fin de Día";
                    alert.TechnicalDetails = exception ?? "No se obtuvo información adicional.";
                }
            }
            catch
            {
                alert.ExecutionDate = today;
                alert.ErrorSummary = message;
                alert.FailedCount = 0;
                alert.PendingCount = 0;
                alert.ProcessedCount = 0;
                alert.ProcessName = "Proceso de Fin de Día";
                alert.TechnicalDetails = exception ?? "No se obtuvo información adicional.";
            }
            

            // Enviar correo de alerta
            await _emailService.SendAsync(new EmailRequest()
            {
                To = technicalEmail,
                Body = _emailHelper.MakeEmailForEodAlert(alert),
                Subject = "Alerta: Problema detectado en proceso de Fin de Día",
            });
        }   
    }
}