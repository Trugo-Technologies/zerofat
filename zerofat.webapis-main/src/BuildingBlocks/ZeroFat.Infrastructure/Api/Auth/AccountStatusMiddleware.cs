using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Shared;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Infrastructure.Api.Auth;

public class AccountStatusMiddleware : IMiddleware
{
    private readonly IClientService _clientService;
    private readonly ICurrentUser _currentUser;

    public AccountStatusMiddleware(
        IClientService clientService,
        ICurrentUser currentUser)
    {
        _clientService = clientService;
        _currentUser = currentUser;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // 1. Skip check for excluded paths (e.g., swagger, health checks).
        if (ExcludePath(context))
        {
            await next(context);
            return;
        }

        // 2. Only check for authenticated users.
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        bool isClient = _currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);

        if (!isClient)
        {
            await next(context);
            return;
        }

        // 3. Get the user's ID from the token claims.
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !DefaultIdType.TryParse(userIdClaim.Value, out var clientId))
        {
            await next(context);
            return;
        }

        // 4. Fetch client from the database.
        var clientIsActive = await _clientService.GetClientStatusByClientId(clientId);

        // 5. Check if the account is active and not deleted.
        if (!clientIsActive)
        {
            // If inactive, block the request.
            await BlockRequestWithProblemDetails(context);
            return; // Stop processing.
        }

        // If checks pass, continue to the next middleware.
        await next(context);
    }

    private static Task BlockRequestWithProblemDetails(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;

        var problemDetails = new ProblemDetails
        {
            // A URI reference that identifies the problem type.
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",

            // A short, human-readable summary of the problem type.
            Title = "Account Inactive or Deleted",

            // The HTTP status code.
            Status = StatusCodes.Status403Forbidden,

            // A human-readable explanation specific to this occurrence of the problem.
            Detail = "Your account is currently inactive or has been deleted. Please contact support for assistance.",

            // A URI reference that identifies the specific occurrence of the problem.
            Instance = context.Request.Path
        };

        return context.Response.WriteAsJsonAsync(problemDetails);
    }


    private bool ExcludePath(HttpContext context)
    {
        // Add any paths you want the middleware to ignore.
        var pathsToExclude = new List<string>
        {
            "/swagger",
            "/health-api",
            "/jobs"
        };

        return pathsToExclude.Any(path => context.Request.Path.StartsWithSegments(path));
    }
}
