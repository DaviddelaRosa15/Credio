namespace Credio.Core.Application.Dtos.Response;

public class SuccessResponse<T>
{
    public string Detail { get; } = "The operation was carried out successfully.";

    public string Type { get; } = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.3.1";

    public int StatusCode { get; } = 200;

    public T? Data { get; set; }
}