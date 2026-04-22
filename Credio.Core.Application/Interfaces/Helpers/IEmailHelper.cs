using Credio.Core.Application.Dtos.CoreConfiguration;
using Credio.Core.Application.Dtos.Email;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Dtos.Payment;

namespace Credio.Core.Application.Interfaces.Helpers
{
    public interface IEmailHelper
    {
        string MakeEmailForChange(string fullName);
        string MakeEmailForEmployee(EmployeeWelcomeEmail employee);
        string MakeEmailForLoanArrearsNotice(LoanInArrearsNotificationDTO notification);
        string MakeEmailForLoanDisbursement(DisburseLoanNotificationDTO notification);
        string MakeEmailForReset(string fullName, string code);
        string MakeEmailForEodAlert(EodAlertNotificationDTO notification);
        string MakeEmailForPaymentNotifications(PaymentNotificationDTO notification);
    }
}