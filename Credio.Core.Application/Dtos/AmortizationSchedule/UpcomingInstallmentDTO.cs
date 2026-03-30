public class UpcomingInstallmentDTO
{
    public string Client { get; set; }

    public int Loan { get; set; }

    public double DueAmount { get; set; }

    public DateOnly DueDate { get; set; }

    public string State { get; set; }
}