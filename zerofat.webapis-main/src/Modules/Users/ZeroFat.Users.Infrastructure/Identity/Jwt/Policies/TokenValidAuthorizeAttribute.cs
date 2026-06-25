using Microsoft.AspNetCore.Authorization;

namespace ZeroFat.Users.Infrastructure.Auth.Jwt;
public sealed class TokenValidAuthorizeAttribute : AuthorizeAttribute
{
    public TokenValidAuthorizeAttribute(string roles)
    {
        Policy = PolicyNameKeys.TokenValid;
        Roles = roles;
    }
}
