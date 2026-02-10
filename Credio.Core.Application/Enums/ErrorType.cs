namespace Credio.Core.Application.Enums;

public enum ErrorType
{
    None = 0,
    Validation = 1,
    BadRequest = 2,
    NotFound = 3,
    Conflict = 4,
    InternalServerError = 5
}