using System.Text.RegularExpressions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.NutriPlan.Application.MealPlanning.Ingredients;
using ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationGroups;
using ZeroFat.NutriPlan.Application.MealPlanning.Meals;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationOptions;

public class MealCustomizationOptionSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }


    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? ImageUrl { get; set; }
    public decimal PriceAdjustment { get; set; } // Can be positive or negative

    // Nutritional impact
    public double CaloriesAdjustment { get; set; }
    public double FatAdjustment { get; set; }
    public double CarbsAdjustment { get; set; }
    public double ProteinAdjustment { get; set; }


}

public class MealCustomizationOptionRawDto : MealCustomizationOptionSimplifyDto
{
    public DefaultIdType GroupId { get; set; }
    public DefaultIdType IngredientId { get; set; }
    public DefaultIdType? MealId { get; set; } // The base meal this option belongs to

    // Indicates if this is the default selection in its group
    public bool IsDefault { get; set; }
    // For options that replace rather than add to the base (e.g., different protein)
    public bool IsReplacement { get; set; }
    // For replacement options, what they replace (e.g., "Chicken" replaces "Base Protein")
    public string? ReplacesComponent { get; set; }

}

public class MealCustomizationOptionAuditableDto : MealCustomizationOptionRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class MealCustomizationOptionDto : MealCustomizationOptionAuditableDto
{
    public MealSimplifyDto? Meal { get; set; }
    public MealCustomizationGroupSimplifyDto? Group { get; set; }
    public IngredientSimplifyDto? Ingredient { get; set; }
}

public class MealCustomizationOptionDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }

    public DefaultIdType GroupId { get; set; }
    public DefaultIdType IngredientId { get; set; }

    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? ImageUrl { get; set; }
    public decimal PriceAdjustment { get; set; } // Can be positive or negative

    // Nutritional impact
    public double CaloriesAdjustment { get; set; }
    public double FatAdjustment { get; set; }
    public double CarbsAdjustment { get; set; }
    public double ProteinAdjustment { get; set; }
    // Indicates if this is the default selection in its group
    public bool IsDefault { get; set; }
    // For options that replace rather than add to the base (e.g., different protein)
    public bool IsReplacement { get; set; }
    // For replacement options, what they replace (e.g., "Chicken" replaces "Base Protein")
    public string? ReplacesComponent { get; set; }
    public DefaultIdType? MealId { get; set; } // The base meal this option belongs to

    public MealSimplifyDto? Meal { get; set; }
    public MealCustomizationGroupSimplifyDto? Group { get; set; }
    public IngredientSimplifyDto? Ingredient { get; set; }
}

