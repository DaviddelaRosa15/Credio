namespace Credio.Core.Application.Interfaces.Services
{
    public interface IEmailTemplateService
    {
        string RenderTemplate(string templateName, Dictionary<string, string> placeholders);
    }
}
