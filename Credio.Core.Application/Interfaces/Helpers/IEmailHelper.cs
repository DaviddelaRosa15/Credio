using Credio.Core.Application.Dtos.Email;

namespace Credio.Core.Application.Interfaces.Helpers
{
    public interface IEmailHelper
    {
        string MakeEmailForChange(string fullName);
        string MakeEmailForReset(string fullName, string code);
    }
}