namespace ZeroFat.NutriPlan.Domain.Settings;

public class MealPlanMealType : Entity, IAggregateRoot
{
    public MealType MealType { get; set; } = default!;
    public MealPlan MealPlan { get; set; } = default!;
    public decimal? AverageCalories { get; set; }
    public decimal? Price { get; set; }
    public DefaultIdType MealTypeId { get; set; }
    public DefaultIdType MealPlanId { get; set; }
}
