using System.ComponentModel.DataAnnotations;

namespace ZeroFat.Users.Infrastructure.Identity.Jwt.Configuration;

public class JwtOptions : IValidatableObject
{
    public const string SectionName = "SecuritySettings:JwtSettings";
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string? Audience { get; set; }

    public int TokenExpirationInMinutes { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }
    public int LinksExpirationInMinutes { get; set; }

    public bool EnableDefaultOTP { get; set; }
    public string? DefaultOTP { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Key))
        {
            yield return new ValidationResult("No Key defined in JwtSettings config", new[] { nameof(Key) });
        }
    }
}
