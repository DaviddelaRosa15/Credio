using Credio.Core.Application.Dtos.CoreConfiguration;
using Credio.Core.Application.Dtos.Email;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Dtos.Payment;
using Credio.Core.Application.Interfaces.Helpers;
using Credio.Core.Application.Interfaces.Services;

namespace Credio.Core.Application.Helpers
{
    public class EmailHelper : IEmailHelper
    {
        private readonly IEmailTemplateService _service;

        public EmailHelper(IEmailTemplateService service)
        {
            _service = service;
        }

        public static string MakeEmailForConfirmed(string user)
        {
            string htmlBody = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Cuenta Confirmada</title>
    <style>
        /* Estilos adicionales */
        body {
            font-family: Arial, sans-serif;
        }
        .container {
            max-width: 400px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
            border-radius: 5px;
        }
        .header {
            text-align: center;
            margin-bottom: 20px;
        }
        .title {
            font-size: 24px;
            margin-bottom: 10px;
        }
        .message {
            font-size: 16px;
            margin-bottom: 20px;
        }
        .footer {
            text-align: center;
            font-size: 14px;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 class='title'>¡Cuenta Confirmada!</h1>
        </div>
        <div class='message'>
            <p>Hola Nombre,</p>
            <p>Te damos la bienvenida a nuestra comunidad. Gracias por confirmar tu cuenta.</p>
            <p>Si tienes alguna pregunta o necesitas ayuda, no dudes en contactarnos.</p>
            <p>¡Disfruta de la aplicación y que tengas un gran día!</p>
        </div>
        <div class='footer'>
            <p>Atentamente,</p>
            <p>El equipo de Base</p>
        </div>
    </div>
</body>
</html>
";
            string html = htmlBody.Replace("Nombre", user);
            return html;
        }

        public string MakeEmailForChange(string fullName)
        {
            return _service.RenderTemplate("PasswordChanged", new Dictionary<string, string>
            {
                { "FullName", fullName },
            });
        }

        public string MakeEmailForEmployee(EmployeeWelcomeEmail employee)
        {
            return _service.RenderTemplate("EmployeeWelcome", new Dictionary<string, string>
            {
                { "FullName", employee.FullName },
                { "UserName", employee.UserName },
                { "Password", employee.Password },
                { "Role", employee.Role }
            });
        }

        public string MakeEmailForLoanArrearsNotice(LoanInArrearsNotificationDTO notification)
        {
            return _service.RenderTemplate("LoanArrearsNotice", new Dictionary<string, string>
            {
                { "ArrearsAmount", $"RD$ {notification.ArrearsAmount:N2}" },
                { "ClientName", notification.ClientName },
                { "DaysOverdue", notification.DaysOverdue.ToString() },
                { "LoanNumber", notification.LoanNumber.ToString() },
                { "TotalDue", $"RD$ {notification.TotalDue:N2}" }
            });
        }

        public string MakeEmailForLoanDisbursement(DisburseLoanNotificationDTO notification)
        {
            return _service.RenderTemplate("LoanDisbursement", new Dictionary<string, string>
            {
                { "ClientName", notification.ClientName },
                { "LoanNumber", notification.LoanNumber.ToString() },
                { "DisbursementDate", notification.EffectiveDate.ToString() },
                { "DisbursedAmount", $"RD$ {notification.LoanAmount:N2}" }
            });
        }

        public string MakeEmailForReset(string fullName, string code)
        {
            return _service.RenderTemplate("ConfirmationCode", new Dictionary<string, string>
            {
                { "FullName", fullName },
                { "ConfirmationCode", code }
            });
        }

        public string MakeEmailForEodAlert(EodAlertNotificationDTO notification)
        {
            return _service.RenderTemplate("SystemErrorAlert", new Dictionary<string, string>
            {
                { "ErrorSummary", notification.ErrorSummary },
                { "ExecutionDate", notification.ExecutionDate.ToString("dd/MM/yyyy")  },
                { "FailedCount", notification.FailedCount.ToString() },
                { "PendingCount", notification.PendingCount.ToString() },
                { "ProcessedCount", notification.ProcessedCount.ToString() },
                { "ProcessName", notification.ProcessName },
                { "TechnicalDetails", notification.TechnicalDetails }
            });
        }

        public string MakeEmailForPaymentNotifications(PaymentNotificationDTO notification)
        {
            return _service.RenderTemplate("PaymentReceipt", new Dictionary<string, string>
            {
                { "ClientName", notification.ClientName },
                { "LoanNumber", notification.LoanNumber.ToString() },
                { "PaymentDate", notification.PaymentDate.ToString() },
                { "PaidAmount", $"RD$ {notification.AmountPaid:N2}" },
                { "RemainingBalance", $"RD$ {notification.RemainingBalance:N2}" }
            });
        }
    }
}