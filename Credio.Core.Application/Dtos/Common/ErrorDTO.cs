namespace Credio.Core.Application.Dtos.Common
{
    public class ErrorDTO
    {
        public string Status { get; set; }
        public List<ErrorDetailsDTO> Details { get; set; }
    }
}
