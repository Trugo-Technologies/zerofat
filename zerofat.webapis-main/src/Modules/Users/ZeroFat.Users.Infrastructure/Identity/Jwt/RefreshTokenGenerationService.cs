using ZeroFat.Domain.Common;
using ZeroFat.Users.Application.Contracts;
using ZeroFat.Users.Infrastructure.Identity.Jwt.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using Microsoft.IdentityModel.Tokens;

namespace ZeroFat.Users.Infrastructure.Auth.Jwt;

internal sealed class RefreshTokenGenerationService : IRefreshTokenGenerationService
{
    private readonly JwtOptions _jwtOptions;
    private readonly IReadRepository<Device> _deviceRepository;

    public RefreshTokenGenerationService(IOptions<JwtOptions> jwtOptions, IReadRepository<Device> deviceRepository)
    {
        _jwtOptions = jwtOptions.Value;
        _deviceRepository = deviceRepository;
    }

    public string GenerateRefreshTokenAsync(Device device)
    {
        device.RefreshToken = GenerateRandomRefreshToken();
        device.RefreshTokenExpiryTime = SystemTime.Now().AddDays(_jwtOptions.RefreshTokenExpirationInDays);

        return device.RefreshToken;
    }

    public async Task<bool> HasValidRefreshTokenAsync(Guid userPublicId, string refreshToken)
    {
        var userRefreshToken = await _deviceRepository.FirstOrDefaultAsync(new ExpressionSpecification<Device>(x => x.RefreshToken == refreshToken && x.UserPublicId == userPublicId));
        if (userRefreshToken == null)
        {
            return false;
        }

        return userRefreshToken.RefreshTokenExpiryTime > SystemTime.Now();
    }

    private static string GenerateRandomRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        var refreshTokenString = Convert.ToBase64String(bytes);

        return refreshTokenString;
    }
}
