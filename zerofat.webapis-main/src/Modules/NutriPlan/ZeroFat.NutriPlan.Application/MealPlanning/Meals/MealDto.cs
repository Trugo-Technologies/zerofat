using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Application.MealPlanning.Recipes;
using ZeroFat.NutriPlan.Application.Settings.Allergens;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Meals;

public class MealSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameEn { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public string? NameAr { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public string? ImageUrl { get; set; }

    public string? PreparationMethodEn { get; set; }
    public string? FullRecipeTextEn { get; set; }


    public CuisineType? Cuisine { get; set; }
    public bool IsAddOn { get; set; }
    public double PriceForCustomer { get; set; }

    public bool IsGlutenFree { get; set; }
    public bool IsDairyFree { get; set; }
    public bool IsLowGI { get; set; }
    public bool IsCold { get; set; }
    public bool IsWarm { get; set; }
    public bool IsActive { get; set; }
    /// <summary>
    /// Is the recipe suitable for freezing.
    /// </summary>
    public bool SuitableForFreezing { get; set; }
    public DefaultIdType? OrginalMealId { get; set; }

}

public class MealRawDto : MealSimplifyDto
{
    public DefaultIdType? RecipeId { get; set; } // Foreign key to the recipe
    public double WeightInGrams { get; set; }
    public double Calories { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
    public double Protein { get; set; }
    public double Water { get; set; }
    public List<DietaryCategory>? DietaryCategories { get; set; }
}

public class MealAuditableDto : MealRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class MealDto : MealAuditableDto
{
    public List<AllergenSimplifyDto> Allergens { get; set; } = [];
}

public class MealDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameEn { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public string? NameAr { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public string? ImageUrl { get; set; }

    public string? PreparationMethodEn { get; set; }
    public string? PreparationMethodAr { get; set; }
    public string? FullRecipeTextEn { get; set; }
    public string? FullRecipeTextAr { get; set; } 


    public CuisineType? Cuisine { get; set; }
    public bool IsAddOn { get; set; }
    public double PriceForCustomer { get; set; }

    public bool IsGlutenFree { get; set; }
    public bool IsDairyFree { get; set; }
    public bool IsLowGI { get; set; }
    public bool IsCold { get; set; }
    public bool IsWarm { get; set; }
    /// <summary>
    /// Is the recipe suitable for freezing.
    /// </summary>
    public bool SuitableForFreezing { get; set; }

    public DefaultIdType? RecipeId { get; set; } // Foreign key to the recipe
    public double WeightInGrams { get; set; }
    public double Calories { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
    public double Protein { get; set; }
    public double Water { get; set; }
    public List<DietaryCategory>? DietaryCategories { get; set; }
    public RecipeSimplifyDto? Recipe { get; set; }
    public List<Allergen>? Allergens { get; set; }

}
