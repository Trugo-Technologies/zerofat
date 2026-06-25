using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Shared;
using ZeroFat.Domain.Enums;
using ZeroFat.Users.Application.Users;

namespace ZeroFat.Users.Application.Authentication.DTOs;
public class TokenResponse
{
    public TokenResponse(string token, string refreshToken, DateTime refreshTokenExpiryTime, UserDto user)
    {
        Token = token;
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = refreshTokenExpiryTime;
        User = user;
    }
    public TokenResponse(bool requiresTwoFactor, string twoFactorToken)
    {
        RequiresTwoFactor = requiresTwoFactor;
        TwoFactorToken = twoFactorToken;
    }

    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public UserDto? User { get; set; }
    public ClientSharedDto? Client { get; set; }
    public ClientSubscriptionSharedDto? ClientSubscription { get; set; }

    public string? TwoFactorToken { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public ClientPaymentMethodShareDto? DefaultPaymentMethod { get; set; }
}

public class MeResponse
{
    public MeResponse(UserDto user)
    {
        User = user;
    }
    public UserDto? User { get; set; }
    public ClientSharedDto? Client { get; set; }
    public ClientSubscriptionSharedDto? ClientSubscription { get; set; }
    public ClientPaymentMethodShareDto? DefaultPaymentMethod { get; set; }
}


