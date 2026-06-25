using ZeroFat.Domain.Enums;

namespace ZeroFat.NutriPlan.Domain.MealPlanning;

public class RecipeIngredient : Entity, IAggregateRoot
{
    public bool HideOnCustomerPorter { get; set; }

    /// <summary>
    /// Amount of the ingredient required for the recipe.
    /// </summary>
    public double Amount { get; set; }
    /// <summary>
    /// Equivalent weight of the ingredient in grams.
    /// </summary>
    public double WeightInGrams { get; set; }
    /// <summary>
    /// Indicates if the ingredient is optional in the recipe.
    /// </summary>
    public bool IsOptional { get; set; }

    public BasicUnit BasicUnit { get; set; }

    public DefaultIdType IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = default!;
    public DefaultIdType RecipeId { get; set; }
    public Recipe Recipe { get; set; } = default!;

    public string? MeasurementUnitCode { get; set; }
}
