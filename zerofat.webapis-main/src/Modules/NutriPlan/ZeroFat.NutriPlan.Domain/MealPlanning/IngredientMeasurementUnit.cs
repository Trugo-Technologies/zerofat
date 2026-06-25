namespace ZeroFat.NutriPlan.Domain.MealPlanning;

public class IngredientMeasurementUnit : Entity, IAggregateRoot
{
    public DefaultIdType? IngredientId { get; set; }
    public Ingredient? Ingredient { get; set; }

    public string? Code { get; set; }
    public double EquivalentInUnit { get; set; }
    public bool IsDefault { get; set; }
}
