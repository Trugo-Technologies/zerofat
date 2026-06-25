namespace ZeroFat.NutriPlan.Domain.Settings;

public class NutrientsAttribute : AuditableEntity, IAggregateRoot
{
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? Unit { get; set; } // g - MG -UG -
}

