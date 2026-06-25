using System.Security.Claims;

namespace ZeroFat.Application.Common.Interfaces;

public interface ICurrentUser
{
    string? Name { get; }

    Guid GetUserId();
    Guid? GetDeviceId();

    string? GetUserEmail();

    string? GetRoleType();
    string? GetPhoneNumber();

    bool IsAuthenticated();

    bool IsInRole(string role);

    IEnumerable<Claim>? GetUserClaims();
}
