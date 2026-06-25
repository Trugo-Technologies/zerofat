using Microsoft.AspNetCore.Authorization;

namespace ZeroFat.Users.Infrastructure.Auth.Jwt;

public class NotSameUserRequirement : IAuthorizationRequirement
{
    public NotSameUserRequirement() { }
}
