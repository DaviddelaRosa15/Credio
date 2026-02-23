namespace Credio.Core.Application.Dtos.Email
{
    public class EmployeeWelcomeEmail
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string TemporaryPassword { get; set; }
        public string Role { get; set; }
    }
}
