using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Application.MealPlanning.CustomMeals;
using ZeroFat.NutriPlan.Application.MealPlanning.Meals;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenuMeals;

public class DailyMenuMealSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType? MealId { get; set; } // Foreign key to the recipe
    public bool IsDefault { get; set; }
}

public class DailyMenuMealRawDto : DailyMenuMealSimplifyDto
{
    public DefaultIdType DailyMenuId { get; set; } // Foreign key to the recipe
}

public class DailyMenuMealAuditableDto : DailyMenuMealRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class DailyMenuMealDto : DailyMenuMealAuditableDto
{
    public MealSimplifyDto? Meal { get; set; }
}

public class DailyMenuMealDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }
    public bool IsDefault { get; set; }

    public DefaultIdType? MealId { get; set; } // Foreign key to the recipe
    public DefaultIdType DailyMenuId { get; set; } // Foreign key to the recipe

    public MealSimplifyDto? Meal { get; set; }
}

public class SelecteabeleMealPlanMenuDto : IDto
{
    public Guid Id { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? ImageUrl { get; set; }

    public DietitianGoal DefaultDietitianGoal { get; set; }
    public MealSelectionType Type { get; set; }

    public decimal? CarbPercentage { get; set; }
    public decimal? ProteinPercentage { get; set; }
    public decimal? FatPercentage { get; set; }

    public List<MenuMealDetailsDto> Meals { get; set; } = [];
    public List<CustomMealSimplifyDto> CustomMeals { get; set; } = [];
}

public class MenuMealDto : IDto
{
    public DefaultIdType Id { get; set; }
    public bool IsDefault { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? ImageUrl { get; set; }
    public DateOnly? Date { get; set; }
    public MealSimplifyDto? Meal { get; set; }

}

public class MenuMealDetailsDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType? MealPlanId { get; set; }
    public MealDto? Meal { get; set; }
    public decimal? DefaultPrice { get; set; }
}
