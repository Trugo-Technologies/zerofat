using System.Security.Claims;
using ZeroFat.Users.Infrastructure.Identity.Jwt.Claims;

namespace ZeroFat.Infrastructure.Api.Auth;

public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.Email);

    public static string? GetFullName(this ClaimsPrincipal principal)
        => principal?.FindFirst(InnovateProClaimsTypes.FullName)?.Value;

    public static string? GetFirstName(this ClaimsPrincipal principal)
        => principal?.FindFirst(ClaimTypes.Name)?.Value;

    public static string? GetSurname(this ClaimsPrincipal principal)
        => principal?.FindFirst(ClaimTypes.Surname)?.Value;

    public static string? GetPhoneNumber(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.MobilePhone);

    public static string? GetRoleType(this ClaimsPrincipal principal)
    => principal.FindFirstValue(InnovateProClaimsTypes.RoleType);

    public static string? GetDeviceId(this ClaimsPrincipal principal)
  => principal.FindFirstValue(InnovateProClaimsTypes.DeviceId);

    public static string? GetUserId(this ClaimsPrincipal principal)
       => principal.FindFirstValue(ClaimTypes.NameIdentifier);

    private static string? FindFirstValue(this ClaimsPrincipal principal, string claimType) =>
        principal is null
            ? throw new ArgumentNullException(nameof(principal))
            : principal.FindFirst(claimType)?.Value;
}
