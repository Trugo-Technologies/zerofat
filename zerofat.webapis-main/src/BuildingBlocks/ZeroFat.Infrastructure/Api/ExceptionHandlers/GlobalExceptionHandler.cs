using FluentValidation;
using ZeroFat.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace ZeroFat.Infrastructure.Api.ExceptionHandlers;
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        var problemDetails = new ProblemDetails();
        problemDetails.Instance = httpContext.Request.Path;
        problemDetails.Type = exception.GetType().ToString();

        if (exception is ValidationException fluentException)
        {
            problemDetails.Title = "one or more validation errors occurred.";
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            List<string> validationErrors = new List<string>();
            foreach (var error in fluentException.Errors)
            {
                validationErrors.Add(error.ErrorMessage);
            }
            problemDetails.Extensions.Add("errors", validationErrors);
        }
        else if (exception is CustomException e)
        {
            httpContext.Response.StatusCode = (int)e.StatusCode;
            problemDetails.Title = e.Message;
            if (e.ErrorMessages != null && e.ErrorMessages.Any())
            {
                problemDetails.Extensions.Add("errors", e.ErrorMessages);
            }
        }

        else
        {
            problemDetails.Title = exception.Message;
        }

        LogContext.PushProperty("StackTrace", exception.StackTrace);
        _logger.LogError("{ProblemDetailsTitle}", problemDetails.Title);

        problemDetails.Status = httpContext.Response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);

        return true;
    }
}
