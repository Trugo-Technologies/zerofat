using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Domain.MealPlanning;

public class Recipe : AuditableEntity, IAggregateRoot
{
    public Recipe()
    {
        RecipeMealTypes = [];
        RecipeIngredients = [];
        Tags = [];
        DietaryCategories = [];
    }
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

    public bool IsGlutenFree { get; set; }
    public bool IsDairyFree { get; set; }
    public bool IsLowGI { get; set; }
    public string? ImageUrl { get; set; }

    public bool IsCold { get; set; }
    public bool IsWarm { get; set; }
    public List<string> Tags { get; set; }


    public double WeightInGrams { get; set; }
    public double Calories { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
    public double Protein { get; set; }
    public double Water { get; set; }

    public DefaultIdType? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    public virtual List<RecipeMealType> RecipeMealTypes { get; set; }
    public virtual List<RecipeIngredient> RecipeIngredients { get; set; }
    public virtual List<Meal> Meals { get; set; }

    /// <summary>
    /// Dietary categories like Vegan, Vegetarian.
    /// </summary>
    public List<DietaryCategory> DietaryCategories { get; set; }
}


//public class NutritionInfo
//{
//    /// <summary>
//    /// Calories per serving.
//    /// </summary>
//    public double Calories { get; set; }

//    /// <summary>
//    /// Protein per serving.
//    /// </summary>
//    public double Protein { get; set; }

//    /// <summary>
//    /// Carbs per serving.
//    /// </summary>
//    public double Carbs { get; set; }

//    /// <summary>
//    /// Fat per serving.
//    /// </summary>
//    public double Fat { get; set; }

//    /// <summary>
//    /// Fiber per serving.
//    /// </summary>
//    public double Fiber { get; set; }
//}
