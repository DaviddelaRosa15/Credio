namespace Credio.Core.Application.Dtos.CoreConfiguration
{
    public class EodAlertNotificationDTO
    {
        public string ErrorSummary { get; set; }
        public DateOnly ExecutionDate { get; set; }
        public int FailedCount { get; set; }
        public int PendingCount { get; set; }
        public int ProcessedCount { get; set; }
        public string ProcessName { get; set; }
        public string TechnicalDetails { get; set; }
    }
}