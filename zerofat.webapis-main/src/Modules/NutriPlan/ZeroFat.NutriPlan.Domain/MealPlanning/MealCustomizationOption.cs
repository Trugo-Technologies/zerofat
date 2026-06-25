namespace ZeroFat.NutriPlan.Domain.MealPlanning;

public class MealCustomizationOption : AuditableEntity
{
    public DefaultIdType GroupId { get; set; }
    public virtual MealCustomizationGroup? Group { get; set; }

    public DefaultIdType IngredientId { get; set; }
    public virtual Ingredient? Ingredient { get; set; }

    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? ImageUrl { get; set; }

    public decimal PriceAdjustment { get; set; } // Can be positive or negative

    // Nutritional impact
    public double CaloriesAdjustment { get; set; }
    public double FatAdjustment { get; set; }
    public double CarbsAdjustment { get; set; }
    public double ProteinAdjustment { get; set; }

    // Indicates if this is the default selection in its group
    public bool IsDefault { get; set; }

    // For options that replace rather than add to the base (e.g., different protein)
    public bool IsReplacement { get; set; }

    // For replacement options, what they replace (e.g., "Chicken" replaces "Base Protein")
    public string? ReplacesComponent { get; set; }

    public DefaultIdType? MealId { get; set; } // The base meal this option belongs to
    public virtual Meal? Meal { get; set; }
}
