using Microsoft.AspNetCore.Authorization;

namespace ZeroFat.Users.Infrastructure.Auth.Jwt;
public sealed class SameUserAttribute : AuthorizeAttribute
{
    public SameUserAttribute()
    {
        Policy = PolicyNameKeys.SameUser;
    }
}
