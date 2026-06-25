using System.Security.Claims;

namespace ZeroFat.Infrastructure.Api.Auth;

public interface ICurrentUserInitializer
{
    void SetCurrentUser(ClaimsPrincipal user);

    void SetCurrentUserId(string userId);
}
