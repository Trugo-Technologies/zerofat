using Microsoft.AspNetCore.Http;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Application.Settings.MealTypes;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealPlans;

public class MealPlanSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? Code { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Images { get; set; }

    public DietitianGoal DefaultDietitianGoal { get; set; }

    public decimal? CarbPercentage { get; set; }
    public decimal? ProteinPercentage { get; set; }
    public decimal? FatPercentage { get; set; }
    public bool IsActive { get; set; }
    public string? StripeId { get; set; }
}

public class MealPlanRawDto : MealPlanSimplifyDto
{
}

public class MealPlanAuditableDto : MealPlanRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class MealPlanDto : MealPlanAuditableDto
{
    public List<MealPlanMealTypeDto>? MealPlanMealTypes { get; set; }
}

public class MealPlanDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? Code { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Images { get; set; }

    public DietitianGoal DefaultDietitianGoal { get; set; }

    public decimal? CarbPercentage { get; set; }
    public decimal? ProteinPercentage { get; set; }
    public decimal? FatPercentage { get; set; }
    public string? StripeId { get; set; }
    public List<MealPlanMealTypeDto>? MealPlanMealTypes { get; set; }
}



public class MealPlanMealTypeDto : IDto
{
    public decimal? AverageCalories { get; set; }
    public decimal? Price { get; set; }
    public MealTypeSimplifyDto? MealType { get; set; }
}
