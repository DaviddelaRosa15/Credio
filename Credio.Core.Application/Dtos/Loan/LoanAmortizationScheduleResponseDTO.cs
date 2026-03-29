using Credio.Core.Application.Dtos.Common;

namespace Credio.Core.Application.Dtos.Loan
{
    public class LoanAmortizationScheduleResponseDTO
    {
        public string ClientFullName { get; set; }
        public string LoanId { get; set; }
        public int LoanNumber { get; set; }
        public double PrincipalAmount { get; set; }
        public List<InstallmentDTO> AmortizationSchedule { get; set; } = new List<InstallmentDTO>();
    }
}
