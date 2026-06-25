using Microsoft.AspNetCore.Authorization;

namespace ZeroFat.Users.Infrastructure.Auth.Jwt;
internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    // private readonly IUserService _userService;

    public PermissionAuthorizationHandler() { }
        // IUserService userService _userService = userService;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // if (context.User?.GetUserId() is { } userId && await _userService.HasPermissionAsync(userId, requirement.Permission))
        // {
        //     context.Succeed(requirement);
        // }

        if (context.User.Identity.IsAuthenticated)
        {
            context.Succeed(requirement);
        }
    }
}
