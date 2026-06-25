using Microsoft.EntityFrameworkCore;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Domain.MenuPlanning;

[Index(nameof(Date))]
public class DailyMenu : AuditableEntity, IAggregateRoot
{
    public DailyMenu()
    {
        Meals = [];
    }
    public DateOnly Date { get; set; } // The date for which this daily menu applies
    public decimal? AverageCalories { get; set; }
    public decimal? Price { get; set; }

    public DefaultIdType? MealPlanId { get; set; } // The meal plan associated with this daily menu (e.g., Low Carb)
    public DefaultIdType? MealTypeId { get; set; } // The meal type (e.g., Breakfast, Lunch, Dinner)
    public DefaultIdType? MenuId { get; set; } // The meal type (e.g., Breakfast, Lunch, Dinner)

    public virtual MealPlan? MealPlan { get; set; } // The meal plan associated with this daily menu (e.g., Low Carb)
    public virtual MealType? MealType { get; set; } // The meal type (e.g., Breakfast, Lunch, Dinner)
    public virtual Menu? Menu { get; set; } // The meal type (e.g., Breakfast, Lunch, Dinner)

    public ICollection<DailyMenuMeal> Meals { get; set; } // List of meals available for this day
}
