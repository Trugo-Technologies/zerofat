namespace ZeroFat.NutriPlan.Domain.MealPlanning;

public class MealCustomizationGroup : AuditableEntity
{
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; } // Whether user must select from this group
    public int MinSelection { get; set; } // Minimum options to select
    public int? MaxSelection { get; set; } // Maximum options allowed (0 for unlimited)

    public double CaloriesAdjustment { get; set; }
    public double FatAdjustment { get; set; }
    public double CarbsAdjustment { get; set; }
    public double ProteinAdjustment { get; set; }

    public virtual ICollection<MealCustomizationOption> Options { get; set; } = [];
}
