namespace Credio.Core.Application.Dtos.CoreConfiguration
{
    public class EndOfDayProcessResponseDTO
    {
        public string LogId { get; set; }
        public string Status { get; set; }
        public int TotalLoans { get; set; }
        public int ProcessedLoans { get; set; }
        public int FailedLoans { get; set; }
        public DateTime ExecutionTime { get; set; }
    }
}
