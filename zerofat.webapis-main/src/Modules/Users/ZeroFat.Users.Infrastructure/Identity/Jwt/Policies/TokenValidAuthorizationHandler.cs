using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ZeroFat.Users.Infrastructure.Auth.Jwt;
public class TokenValidAuthorizationHandler : AuthorizationHandler<TokenValidRequirement>
{
    // private readonly ITokenStoreManager tokenStoreManager;
    // ITokenStoreManager tokenStoreManager;

    public TokenValidAuthorizationHandler()
    {
        // this.tokenStoreManager = tokenStoreManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenValidRequirement requirement)
    {
        // && await tokenStoreManager.IsCurrentTokenActiveAsync()
        if (context.User.Identity.IsAuthenticated)
        {
            context.Succeed(requirement);
        }
    }
}
