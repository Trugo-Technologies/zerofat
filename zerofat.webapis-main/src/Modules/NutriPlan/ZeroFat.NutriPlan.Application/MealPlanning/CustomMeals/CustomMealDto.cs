using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Application.Settings.Allergens;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MealPlanning.CustomMeals;

public class CustomMealSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? SpecialInstructions { get; set; }
    public string? ImageUrl { get; set; }


    // Reference to the original meal this was customized from
    public DefaultIdType? BaseMealId { get; set; }

    // The customer who created this custom meal
    public DefaultIdType ClientId { get; set; }

    // Calculated properties
    public double TotalCalories { get; set; }
    public double TotalFat { get; set; }
    public double TotalCarbs { get; set; }
    public double TotalProtein { get; set; }
    public decimal TotalPrice { get; set; }

}

public class CustomMealRawDto : CustomMealSimplifyDto
{
}

public class CustomMealAuditableDto : CustomMealRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class CustomMealDto : CustomMealAuditableDto
{
}

public class CustomMealDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }

}
