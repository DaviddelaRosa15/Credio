using Credio.Core.Application.Interfaces.Services;

namespace Credio.Infrastructure.Shared.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string RenderTemplate(string templateName, Dictionary<string, string> placeholders)
        {
            var templatePath = Path.Combine(AppContext.BaseDirectory, "EmailTemplates", templateName + ".html");
            string html = File.ReadAllText(templatePath);

            if (placeholders != null)
            {
                foreach (var kvp in placeholders)
                {
                    html = html.Replace("{{" + kvp.Key + "}}", kvp.Value);
                }
            }

            return html;
        }
    }
}
