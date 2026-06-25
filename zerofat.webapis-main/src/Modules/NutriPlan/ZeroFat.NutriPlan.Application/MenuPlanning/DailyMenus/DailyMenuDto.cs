using ZeroFat.Application.Common.Interfaces;
using ZeroFat.NutriPlan.Application.Settings.MealPlans;
using ZeroFat.NutriPlan.Application.Settings.MealTypes;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenus;

public class DailyMenuSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DateOnly Date { get; set; } // The date for which this daily menu applies
    public decimal? AverageCalories { get; set; }
    public decimal? Price { get; set; }
}

public class DailyMenuRawDto : DailyMenuSimplifyDto
{
    public DefaultIdType? MealPlanId { get; set; } // The meal plan associated with this daily menu (e.g., Low Carb)
    public DefaultIdType? MealTypeId { get; set; } // The meal type (e.g., Breakfast, Lunch, Dinner)
}

public class DailyMenuAuditableDto : DailyMenuRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class DailyMenuDto : DailyMenuAuditableDto
{
    public MealPlanSimplifyDto? MealPlan { get; set; }
    public MealTypeSimplifyDto? MealType { get; set; }
}

public class DailyMenuDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }

    public DateOnly Date { get; set; } // The date for which this daily menu applies
    public decimal? AverageCalories { get; set; }
    public decimal? Price { get; set; }

    public DefaultIdType? MealPlanId { get; set; } // The meal plan associated with this daily menu (e.g., Low Carb)
    public DefaultIdType? MealTypeId { get; set; } // The meal type (e.g., Breakfast, Lunch, Dinner)

    public MealPlanSimplifyDto? MealPlan { get; set; }
    public MealTypeSimplifyDto? MealType { get; set; }
}


