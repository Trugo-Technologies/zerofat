using Microsoft.AspNetCore.Authorization;

namespace ZeroFat.Users.Infrastructure.Auth.Jwt;
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string action, string resource)
    {
        Policy = PolicyNameKeys.HasPermission;
    }
}
