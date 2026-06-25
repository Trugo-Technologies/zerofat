using System.Security.Claims;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.Infrastructure.Api.Auth;

public class CurrentUser : ICurrentUser, ICurrentUserInitializer
{
    private ClaimsPrincipal? _user;

    public string? Name => _user?.Identity?.Name;

    private Guid _userId = Guid.Empty;

    public Guid GetUserId() =>
        IsAuthenticated()
            ? Guid.Parse(_user?.GetUserId() ?? Guid.Empty.ToString())
            : _userId;

    public string? GetUserEmail() =>
        IsAuthenticated()
            ? _user!.GetEmail()
            : string.Empty;


    public Guid? GetDeviceId() =>
    IsAuthenticated()
        ? Guid.Parse(_user?.GetDeviceId() ?? Guid.Empty.ToString())
        : null;

    public string? GetRoleType() =>
    IsAuthenticated()
        ? _user!.GetRoleType()
        : string.Empty;

    public string? GetPhoneNumber() =>
   IsAuthenticated()
       ? _user!.GetPhoneNumber()
       : string.Empty;

    public string? GetUserFullName() =>
        IsAuthenticated()
            ? _user!.GetFullName() ?? _user!.GetEmail()
            : string.Empty;


    public bool IsAuthenticated() =>
        _user?.Identity?.IsAuthenticated is true;

    public bool IsInRole(string role) =>
        _user?.IsInRole(role) is true;

    public IEnumerable<Claim>? GetUserClaims() =>
        _user?.Claims;

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        if (_user != null)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        _user = user;
    }

    public void SetCurrentUserId(string userId)
    {
        if (_userId != Guid.Empty)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        if (!string.IsNullOrEmpty(userId))
        {
            _userId = Guid.Parse(userId);
        }
    }
}
