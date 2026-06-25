using System.Security.Claims;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Users.Domain.Users;

namespace ZeroFat.Users.Application.Contracts;

public interface IJwtTokenManagementService: ITransientService
{
    string GenerateJwtToken(ApplicationUser user, Device device, IList<string> roles);
    Guid GetUserPublicIdFromToken(string accessToken);
    string? GetDefaulOTPToken();
    public string GetUserIdPrincipalFromExpiredToken(string token);
}
