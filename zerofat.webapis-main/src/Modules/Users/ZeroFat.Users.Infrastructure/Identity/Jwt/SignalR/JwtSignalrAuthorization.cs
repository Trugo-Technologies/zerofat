using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ZeroFat.Users.Infrastructure.Identity.Jwt.SignalR;

internal static class JwtSignalrAuthorization
{
    private const string AccessTokenParamName = "access_token";
    private const string Hubs = "/hubs/";
    private static string GetBearerToken(string token)
    {
        return $"Bearer {token}";
    }

    internal static Task ReadTokenFromQueryString(MessageReceivedContext context)
    {
        var token = context.Request.Query[AccessTokenParamName];

        var path = context.HttpContext.Request.Path;
        var signalrRequest = IsSignalrRequest(token, path);
        if (signalrRequest)
        {
            context.Token = GetBearerToken(token);
        }

        return Task.CompletedTask;
    }

    private static bool IsSignalrRequest(StringValues token, PathString path)
    {
        return !string.IsNullOrEmpty(token) && path.StartsWithSegments(Hubs);
    }
}
