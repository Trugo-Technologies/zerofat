using Microsoft.AspNetCore.Authorization;

namespace ZeroFat.Users.Infrastructure.Auth.Jwt;
public sealed class NotSameUserAttribute : AuthorizeAttribute
{
    public NotSameUserAttribute()
    {
        Policy = PolicyNameKeys.NotSameUser;
    }
}
