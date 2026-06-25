using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Application.MealPlanning.RecipeIngredients;
using ZeroFat.NutriPlan.Application.Settings.Categories;
using ZeroFat.NutriPlan.Application.Settings.MealTypes;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Recipes;

public class RecipeSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    /// <summary>
    /// Preparation time in minutes.
    /// </summary>
    public int PreparationTime { get; set; }
    /// <summary>
    /// Cooking time in minutes.
    /// </summary>
    public int CookingTime { get; set; }
    /// <summary>
    /// Total servings this recipe can provide.
    /// </summary>
    public int Servings { get; set; }
    /// <summary>
    /// Recipe difficulty level.
    /// </summary>
    public RecipeDifficulty Difficulty { get; set; }
    /// <summary>
    /// Cuisine type of the recipe (e.g., Italian, Chinese).
    /// </summary>
    public CuisineType? Cuisine { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsGlutenFree { get; set; }
    public bool IsDairyFree { get; set; }
    public bool IsLowGI { get; set; }
    public bool IsCold { get; set; }
    public bool IsWarm { get; set; }

    public List<string> Tags { get; set; }

    public DefaultIdType? CategoryId { get; set; }
    public List<DietaryCategory> DietaryCategories { get; set; }
}

public class RecipeRawDto : RecipeSimplifyDto
{
    public double WeightInGrams { get; set; }
    public double Calories { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
    public double Protein { get; set; }
    public double Water { get; set; }

}

public class RecipeAuditableDto : RecipeRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class RecipeDto : RecipeAuditableDto
{
    public CategorySimplifyDto? Category { get; set; }
    public List<MealTypeSimplifyDto>? MealTypes { get; set; }
}

public class RecipeDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? PreparationMethodEn { get; set; }
    public string? PreparationMethodAr { get; set; }
    public string? FullRecipeTextEn { get; set; }
    public string? FullRecipeTextAr { get; set; }

    /// <summary>
    /// Preparation time in minutes.
    /// </summary>
    public int PreparationTime { get; set; }
    /// <summary>
    /// Cooking time in minutes.
    /// </summary>
    public int CookingTime { get; set; }
    /// <summary>
    /// Total servings this recipe can provide.
    /// </summary>
    public int Servings { get; set; }
    /// <summary>
    /// Recipe difficulty level.
    /// </summary>
    public RecipeDifficulty Difficulty { get; set; }
    /// <summary>
    /// Cuisine type of the recipe (e.g., Italian, Chinese).
    /// </summary>
    public CuisineType? Cuisine { get; set; }


    public string? ImageUrl { get; set; }

    public bool IsGlutenFree { get; set; }
    public bool IsDairyFree { get; set; }
    public bool IsLowGI { get; set; }
    public bool IsCold { get; set; }
    public bool IsWarm { get; set; }

    public double WeightInGrams { get; set; }
    public double Calories { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
    public double Protein { get; set; }
    public double Water { get; set; }


    public List<string>? Tags { get; set; }

    public DefaultIdType? CategoryId { get; set; }
    public List<DietaryCategory>? DietaryCategories { get; set; }
    public CategorySimplifyDto? Category { get; set; }
    public List<MealTypeSimplifyDto>? MealTypes { get; set; }
    public List<RecipeIngredientDto>? RecipeIngredients { get; set; }
}


