using Credio.Core.Application.Dtos.Common;

namespace Credio.Core.Application.Dtos.LoanApplication
{
    public class SimulationResponseDTO
    {
        public double TotalAmount { get; set; }
        public double TotalInterest { get; set; }
        public double TotalToPay { get; set; }
        public double InstallmentAmount { get; set; }
        public List<InstallmentDTO> Schedule { get; set; }
    }
}