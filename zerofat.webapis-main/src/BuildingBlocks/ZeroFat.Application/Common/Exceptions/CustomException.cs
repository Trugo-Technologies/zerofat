using System.Net;

namespace ZeroFat.Application.Common.Exceptions;

public class CustomException : Exception
{
    public IReadOnlyCollection<string>? ErrorMessages { get; }

    public HttpStatusCode StatusCode { get; }

    public CustomException(string message, IReadOnlyCollection<string>? errors = default, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        ErrorMessages = errors;
        StatusCode = statusCode;
    }
}
