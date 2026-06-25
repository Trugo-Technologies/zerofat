using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Domain.MealPlanning;

public class IngredientAttribute : Entity, IAggregateRoot
{
    public decimal? Value { get; set; }

    public DefaultIdType IngredientId { get; set; }
    public DefaultIdType NutrientsAttributeId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = default!;
    public virtual NutrientsAttribute NutrientsAttribute { get; set; } = default!;
}

