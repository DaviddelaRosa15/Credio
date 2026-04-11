namespace Credio.Core.Application.Dtos.CoreConfiguration
{
    public class LoanInArrearsNotificationDTO
    {
        public double ArrearsAmount { get; set; }
        public int DaysOverdue { get; set; }
        public string ClientName { get; set; }
        public int LoanNumber { get; set; }
        public double TotalDue { get; set; }
    }
}