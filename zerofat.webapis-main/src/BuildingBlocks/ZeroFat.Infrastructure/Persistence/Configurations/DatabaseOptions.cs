using System.ComponentModel.DataAnnotations;

namespace ZeroFat.Infrastructure.Persistence.Configurations;
public class DatabaseOptions : IValidatableObject
{
    public string Provider { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string? DatabaseName { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(ConnectionString))
        {
            yield return new ValidationResult("connection string cannot be empty.", new[] { nameof(ConnectionString) });
        }
    }
}
