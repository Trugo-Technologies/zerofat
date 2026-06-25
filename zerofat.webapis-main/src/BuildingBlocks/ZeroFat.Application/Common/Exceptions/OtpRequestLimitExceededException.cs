using System.Net;

namespace ZeroFat.Application.Common.Exceptions;

public class OtpRequestLimitExceededException : CustomException
{
    public OtpRequestLimitExceededException(string message, List<string> errors)
        : base(message, errors, HttpStatusCode.Forbidden)
    {
    }
}
