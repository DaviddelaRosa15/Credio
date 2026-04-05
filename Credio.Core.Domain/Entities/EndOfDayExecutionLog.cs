using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities
{
    public class EndOfDayExecutionLog : AuditableBaseEntity
    {
        public EndOfDayExecutionLog()
        {
            QueueItems = [];
        }

        public DateOnly ExecutionDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = null!; // "NotStarted", "Processing", "Completed", "Failed", "CompletedWithErrors"
        public int TotalLoans { get; set; }
        public int ProcessedLoans { get; set; }
        public string? Notes { get; set; }

        // Relación con los items de la cola
        public List<EndOfDayQueue> QueueItems { get; set; }
    }
}