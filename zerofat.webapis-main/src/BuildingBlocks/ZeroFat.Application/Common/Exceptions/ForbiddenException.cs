using System.Net;

namespace ZeroFat.Application.Common.Exceptions;

public class ForbiddenException : CustomException
{
    public ForbiddenException(string message)
        : base(message, null, HttpStatusCode.Forbidden)
    {
    }
    public ForbiddenException(string message, List<string> errors)
        : base(message, errors, HttpStatusCode.Forbidden)
    {
    }
}
