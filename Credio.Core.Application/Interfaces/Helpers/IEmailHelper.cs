using Credio.Core.Application.Dtos.CoreConfiguration;
using Credio.Core.Application.Dtos.Email;

namespace Credio.Core.Application.Interfaces.Helpers
{
    public interface IEmailHelper
    {
        string MakeEmailForChange(string fullName);
        string MakeEmailForEmployee(EmployeeWelcomeEmail employee);
        string MakeEmailForLoanArrearsNotice(LoanInArrearsNotificationDTO notification);
        string MakeEmailForReset(string fullName, string code);
        string MakeEmailForEodAlert(EodAlertNotificationDTO notification);
    }
}