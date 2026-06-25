using Microsoft.AspNetCore.Authorization;

namespace ZeroFat.Users.Infrastructure.Auth.Jwt;

internal class SameUserRequirement : IAuthorizationRequirement
{
    public SameUserRequirement() { }
}
