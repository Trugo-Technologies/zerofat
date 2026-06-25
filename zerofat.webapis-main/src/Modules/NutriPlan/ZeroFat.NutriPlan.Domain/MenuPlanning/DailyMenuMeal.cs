using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Domain.MenuPlanning;

public class DailyMenuMeal : AuditableEntity, IAggregateRoot
{
    public bool IsDefault { get; set; }
    public DefaultIdType? MealId { get; set; }
    public DefaultIdType? DailyMenuId { get; set; }

    // Navigation properties
    public virtual Meal? Meal { get; set; } // The recipe for this meal
    public virtual DailyMenu? DailyMenu { get; set; }
}
