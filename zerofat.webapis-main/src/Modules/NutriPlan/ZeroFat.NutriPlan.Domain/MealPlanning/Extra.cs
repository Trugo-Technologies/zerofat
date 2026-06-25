namespace ZeroFat.NutriPlan.Domain.MealPlanning;

public class Extra : AuditableEntity
{
    public string? NameEn { get; set; } // Name of the extra (e.g., "Avocado", "Cheese")
    public string? NameAr { get; set; } // Name of the extra (e.g., "Avocado", "Cheese")
    public decimal Price { get; set; } // Additional price for adding this extra
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Amount of the ingredient required for the recipe.
    /// </summary>
    public double Amount { get; set; }
    /// <summary>
    /// Equivalent weight of the ingredient in grams.
    /// </summary>
    public double WeightInGrams { get; set; }

    public DefaultIdType? OrginalIngredientId { get; set; }

    // Navigation properties
    public DefaultIdType? IngredientId { get; set; }
    public virtual Ingredient? Ingredient { get; set; } = default!;
    public DefaultIdType MealId { get; set; }
    public virtual Meal Meal { get; set; } = default!;
}

