using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Domain.MealPlanning;

public class IngredientAllergen : Entity, IAggregateRoot
{
    public DefaultIdType? IngredientId { get; set; }
    public Ingredient? Ingredient { get; set; }

    public DefaultIdType? AllergenId { get; set; }
    public Allergen? Allergen { get; set; }
}
