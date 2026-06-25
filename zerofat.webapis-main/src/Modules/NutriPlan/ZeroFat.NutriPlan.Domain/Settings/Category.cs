using ZeroFat.Domain.Enums;

namespace ZeroFat.NutriPlan.Domain.Settings;

public class Category : AuditableEntity, IAggregateRoot
{
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? IconUrl { get; set; }
    public string? ImageUrl { get; set; }
    public CategoryType? CategoryType { get; set; }
}
