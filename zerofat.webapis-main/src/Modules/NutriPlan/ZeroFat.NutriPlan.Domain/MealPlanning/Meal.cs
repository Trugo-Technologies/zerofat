using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Domain.MealPlanning;

public class Meal : ActivationEntity, IAggregateRoot
{
    public Meal()
    {
        Extras = [];
        DietaryCategories = [];
        Allergens = [];
    }
    public string? NameEn { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public string? NameAr { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public string? ImageUrl { get; set; }

    public string? PreparationMethodEn { get; set; }
    public string? PreparationMethodAr { get; set; }
    public string? FullRecipeTextEn { get; set; }
    public string? FullRecipeTextAr { get; set; }

    /// <summary>
    /// Dietary categories like Vegan, Vegetarian.
    /// </summary>
    public List<DietaryCategory> DietaryCategories { get; set; }
    // public float PortionSize { get; set; } // Portion size of the meal
    public double WeightInGrams { get; set; }
    public double Calories { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
    public double Protein { get; set; }
    public double Water { get; set; }

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
    public DefaultIdType? OrginalMealId { get; set; }
    public DefaultIdType? ClientId { get; set; }

    // Navigation properties
    public DefaultIdType? RecipeId { get; set; } // Foreign key to the recipe
    public virtual Recipe? Recipe { get; set; } // The recipe for this meal
    public virtual ICollection<Extra> Extras { get; set; } // List of available extras for this meal
    public virtual ICollection<MealAllergen> Allergens { get; set; } // List of available extras for this meal
}

public class MealAllergen : Entity, IAggregateRoot
{
    public DefaultIdType? MealId { get; set; }
    public virtual Meal? Meal { get; set; }

    public DefaultIdType? AllergenId { get; set; }
    public virtual Allergen? Allergen { get; set; }
}



//public class Meal : ActivationEntity, IAggregateRoot
//{
//    public string? NameEn { get; set; }
//    public string? NameAr { get; set; }
//    public string? DescriptionEn { get; set; }
//    public string? DescriptionAr { get; set; }

//    public string? NotesEn { get; set; }
//    public string? NotesAr { get; set; }
//    public string? Code { get; set; }

//    public string? Price { get; set; }

//    public bool IsVegan { get; set; }
//    public bool IsVegetarian { get; set; }
//    public bool IsGlutenFree { get; set; }
//    public bool IsDairyFree { get; set; }
//    public bool IsLowGI { get; set; }
//    public bool IsSweet { get; set; }
//    public bool IsSpicy { get; set; }
//    public bool IsFish { get; set; }
//    public bool IsMeat { get; set; }
//    public bool IsCold { get; set; }
//    public bool IsWarm { get; set; }
//    public string? ImageUrl { get; set; }


//    public virtual List<MealMealType> MealMealTypes { get; set; }
//}

//public class MealMealType : AuditableEntity, IAggregateRoot
//{
//    public DefaultIdType? MealId { get; set; }
//    public Meal? Meal { get; set; }
//    public DefaultIdType? MealTypeId { get; set; }
//    public MealType? MealType { get; set; }
//}
