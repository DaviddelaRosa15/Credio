namespace Credio.Core.Application.Dtos.Payment
{
    public class RegisterPaymentResponseDTO : PaymentDTO
    {
        public DateOnly PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
    }
}