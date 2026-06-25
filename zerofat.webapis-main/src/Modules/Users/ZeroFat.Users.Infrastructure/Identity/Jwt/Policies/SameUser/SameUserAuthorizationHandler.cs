using ZeroFat.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ZeroFat.Users.Infrastructure.Auth.Jwt;

internal class SameUserAuthorizationHandler : AuthorizationHandler<SameUserRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUser _currentUser;

    public SameUserAuthorizationHandler(IHttpContextAccessor httpContextAccessor, ICurrentUser currentUser)
    {
        _httpContextAccessor = httpContextAccessor;
        _currentUser = currentUser;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            return Task.CompletedTask;
        }

        // Assuming the user ID is passed as a route parameter
        var routeData = httpContext.GetRouteData();
        var resourceUserId = routeData.Values["userId"]?.ToString();

        if (resourceUserId == _currentUser.GetUserId().ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
