using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Http;

namespace ZeroFat.Infrastructure.Api.Auth;

public class CurrentUserMiddleware : IMiddleware
{
    private readonly ICurrentUserInitializer _currentUserInitializer;

    public CurrentUserMiddleware(ICurrentUserInitializer currentUserInitializer) =>
        _currentUserInitializer = currentUserInitializer;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!ExcludePath(context))
        {
            _currentUserInitializer.SetCurrentUser(context.User);
        }

        await next(context);
    }

    private bool ExcludePath(HttpContext context)
    {
        var listExclude = new List<string>()
            {
                "/swagger",
                "/jobs",
                "/files",
                "/health-api",
            };

        foreach (string item in listExclude)
        {
            if (context.Request.Path.StartsWithSegments(item))
            {
                return true;
            }
        }

        return false;
    }
}
