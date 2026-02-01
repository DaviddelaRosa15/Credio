using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.Common;

namespace Credio.Core.Application.Helpers
{
    public static class ErrorMapperHelper
    {
        public static ErrorDTO ListError(List<string> message)
        {
            ErrorDTO error = new();
            error.Status = "Fallido";
            error.Details = new();

            foreach (var item in message)
            {
                ErrorDetailsDTO details = new();
                details.Code = ErrorMessages.BadRequest;
                details.Message = item;

                error.Details.Add(details);
            }

            return error;
        }

        public static ErrorDTO Error(string code, string message)
        {
            ErrorDTO error = new();
            error.Status = "Fallido";
            error.Details = [new ErrorDetailsDTO { Code = code, Message = message }];
            
            return error;
        }
    }
}
