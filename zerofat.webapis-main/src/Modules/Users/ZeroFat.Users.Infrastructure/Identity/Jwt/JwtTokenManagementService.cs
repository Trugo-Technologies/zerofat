using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Domain.Common;
using ZeroFat.Users.Application.Contracts;
using ZeroFat.Users.Infrastructure.Identity.Jwt.Claims;
using ZeroFat.Users.Infrastructure.Identity.Jwt.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ZeroFat.Infrastructure.Api.Auth;

namespace ZeroFat.Users.Infrastructure.Identity.Jwt;
internal sealed class JwtTokenManagementService : IJwtTokenManagementService
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenManagementService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5404:Do not disable token validation checks", Justification = "<Pending>")]
    public DefaultIdType GetUserPublicIdFromToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Key)),
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false // we do not validate lifetime - token can be expired and we will generate new one based on refresh token
        };
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var principal = jwtSecurityTokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        var publicUserId = principal.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;

        return DefaultIdType.Parse(publicUserId);
    }

    public string? GetDefaulOTPToken()
    {
        if (_jwtOptions.EnableDefaultOTP)
        {
            return _jwtOptions.DefaultOTP;
        }
        return string.Empty;

    }


    public string GenerateJwtToken(ApplicationUser user, Device device, IList<string> roles)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var claims = GetClaims(user, device, roles);

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Key));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: SystemTime.Now().AddMinutes(_jwtOptions.TokenExpirationInMinutes),
            signingCredentials: signingCredentials);

        return jwtSecurityTokenHandler.WriteToken(token);
    }

    public string GetUserIdPrincipalFromExpiredToken(string token)
    {
        if (string.IsNullOrEmpty(_jwtOptions.Key))
        {
            throw new InvalidOperationException("No Key defined in JwtSettings config.");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedException("Invalid Token.");
        }

        return principal.GetUserId() ?? string.Empty;
    }

    private IEnumerable<Claim> GetClaims(ApplicationUser user, Device device, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            // new(ClaimTypes.NameIdentifier, user.Id),
            // new(ClaimConstants.IpAddress, headers.IPAddress),

            new(JwtRegisteredClaimNames.Sub, user.PublicId.ToString()),
            new(JwtRegisteredClaimNames.Jti, DefaultIdType.NewGuid().ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.PublicId.ToString() ?? string.Empty),

            new(InnovateProClaimsTypes.RoleType, user.UserType.ToString()),
            new(InnovateProClaimsTypes.DeviceId, device.Id.ToString()),
            new(InnovateProClaimsTypes.Platform, device.Platform?.ToString() ?? string.Empty),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return claims;
    }

}

