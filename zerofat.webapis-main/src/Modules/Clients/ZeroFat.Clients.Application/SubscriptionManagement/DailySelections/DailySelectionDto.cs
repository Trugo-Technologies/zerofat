using ZeroFat.Application.Common.Interfaces;
using ZeroFat.ClientPortal.Application.ClientManagement;
using ZeroFat.ClientPortal.Application.ClientManagement.Clients;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.DailyMealSelections;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.DailySelections;

public class DailySelectionSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }

    public DateOnly Date { get; set; } // Date of the meal selection

    public PreferredMealTime DeliveryTime { get; set; } // The client's preferred delivery time for this meal
    public DailySelectionStatus DailySelectionStatus { get; set; }

    public bool HasCutlery { get; set; }
    public double TotalCalories { get; set; } // Total calories in the selected meal
    public double TotalFats { get; set; } // Total fat content in the selected meal
    public double TotalCarbohydrates { get; set; } // Total carbohydrate content in the selected meal
    public double TotalProteins { get; set; } // Total protein content in the selected meal
}

public class DailySelectionRawDto : DailySelectionSimplifyDto
{

}

public class DailySelectionAuditableDto : DailySelectionRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class DailySelectionDto : DailySelectionAuditableDto
{
    public ClientLocationSimplifyDto? ClientLocation { get; set; } // Foreign key to the client
    public ClientSimplifyDto? Client { get; set; } // Foreign key to the client
}

public class DailySelectionDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }

    public DateOnly Date { get; set; } // Date of the meal selection
    public string? Notes { get; set; } // Special notes or preferences for the meal

    public PreferredMealTime DeliveryTime { get; set; } // The client's preferred delivery time for this meal
    public DailySelectionStatus DailySelectionStatus { get; set; }

    public bool HasCutlery { get; set; }

    public double TotalCalories { get; set; } // Total calories in the selected meal
    public double TotalFats { get; set; } // Total fat content in the selected meal
    public double TotalCarbohydrates { get; set; } // Total carbohydrate content in the selected meal
    public double TotalProteins { get; set; } // Total protein content in the selected meal

    public DefaultIdType ClientLocationId { get; set; } // Foreign key to the client
    public ClientLocationSimplifyDto? ClientLocation { get; set; } // Foreign key to the client

    public List<DailySelectionMealTypeDto> MealTypes { get; set; } = [];
}

public class DailySelectionMealTypeDto : IDto
{
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public int? Index { get; set; }
    public List<MealSelectionDto> DailyMealSelections { get; set; } = [];
}



public class MealSelectionDto : IDto
{
    public DefaultIdType Id { get; set; }

    public MealSelectionType MealSelectionType { get; set; }
    public DefaultIdType? CustomMealId { get; set; }
    public int Qty { get; set; }

    // Pricing information
    public decimal BasePrice { get; set; }  // Original subscription price
    public decimal? AdjustedPrice { get; set; } // Final price after modifications
    public string? PriceAdjustmentReason { get; set; }

    public double? TotalCalories { get; set; } // Total calories in the selected meal
    public double? TotalFats { get; set; } // Total fat content in the selected meal
    public double? TotalCarbohydrates { get; set; } // Total carbohydrate content in the selected meal
    public double? TotalProteins { get; set; } // Total protein content in the selected meal

    // Customization options
    public string? CustomeMealName { get; set; }
    public string? SpecialInstructions { get; set; }

    // Status flags
    public bool IsConsumed { get; set; }
    public bool IsPaid { get; set; }

    public MealSimplifyDto? Meal { get; set; } // The meal selected
    public CustomMealSimplifyDto? CustomMeal { get; set; } // The meal selected
    public DefaultIdType? MealTypeId { get; set; } // Foreign key to the client
    public DefaultIdType MealId { get; set; } // Foreign key to the client

}

public class MealSimplifyDto : IDto
{
    public string? NameEn { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public string? NameAr { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public string? ImageUrl { get; set; }

    public string? PreparationMethodEn { get; set; }
    public string? FullRecipeTextEn { get; set; }

    /// <summary>
    /// Dietary categories like Vegan, Vegetarian.
    /// </summary>
    public List<DietaryCategory>? DietaryCategories { get; set; }
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

    // Navigation properties

    public List<AllergenDto> Allergens { get; set; }
}

public class AllergenDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? IconUrl { get; set; }
    public bool? IsAllergic { get; set; }
}
