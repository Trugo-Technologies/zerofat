using ZeroFat.Application.Common.Interfaces;
using ZeroFat.ClientPortal.Application.ClientManagement.Clients;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.DailySelections;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.MealRatings;
using ZeroFat.Domain.Common.Contracts;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.DailyMealSelections;

public class DailyMealSelectionSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType? CustomMealId { get; set; }

    public DateOnly Date { get; set; } // Date of the meal selection
    public MealSelectionType MealSelectionType { get; set; }
    public int Qty { get; set; }

    // Pricing information
    public decimal BasePrice { get; set; }  // Original subscription price
    public decimal? AdjustedPrice { get; set; } // Final price after modifications
    public string? PriceAdjustmentReason { get; set; }

    // Nutritional information(reflects final meal)
    public double TotalCalories { get; set; }
    public double TotalFats { get; set; }
    public double TotalCarbohydrates { get; set; }
    public double TotalProteins { get; set; }

    // Customization options
    public string? CustomeMealName { get; set; }
    public string? SpecialInstructions { get; set; }

    // Status flags
    public bool IsConsumed { get; set; }
    public bool IsPaid { get; set; }
}

public class DailyMealSelectionRawDto : DailyMealSelectionSimplifyDto
{
    public DefaultIdType DailyMenuMealId { get; set; } // Foreign key to the client
    public DefaultIdType DailySelectionId { get; set; } // Foreign key to the client
    public DefaultIdType MealTypeId { get; set; } // Foreign key to the client
}

public class DailyMealSelectionAuditableDto : DailyMealSelectionRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class DailyMealSelectionDto : DailyMealSelectionAuditableDto
{

    public MealSimplifyDto? Meal { get; set; }
    public CustomMealSimplifyDto? CustomMeal { get; set; }
    public ClientSimplifyDto? Client { get; set; }
    public MealRatingSummaryDto? Rating { get; set; }
}

public class DailyMealSelectionDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType? CustomMealId { get; set; }

    public DateOnly Date { get; set; } // Date of the meal selection
    public MealSelectionType MealSelectionType { get; set; }
    public int Qty { get; set; }

    // Pricing information
    public decimal BasePrice { get; set; }  // Original subscription price
    public decimal? AdjustedPrice { get; set; } // Final price after modifications
    public string? PriceAdjustmentReason { get; set; }

    // Nutritional information(reflects final meal)
    public double TotalCalories { get; set; }
    public double TotalFats { get; set; }
    public double TotalCarbohydrates { get; set; }
    public double TotalProteins { get; set; }

    // Customization options
    public string? CustomeMealName { get; set; }
    public string? SpecialInstructions { get; set; }

    // Status flags
    public bool IsConsumed { get; set; }
    public bool IsPaid { get; set; }

    public DefaultIdType DailyMenuMealId { get; set; } // Foreign key to the client
    public DefaultIdType DailySelectionId { get; set; } // Foreign key to the client
    public DefaultIdType MealTypeId { get; set; } // Foreign key to the client

    public MealSimplifyDto? Meal { get; set; }
    public CustomMealDto? CustomMeal { get; set; }
    public ClientSimplifyDto? Client { get; set; }
}



public class CustomizeMealDto : IDto
{
    public decimal TotalCost { get; set; } // Total cost of the subscription
    public string? PaymentQuickLink { get; set; }
    public string? PaymentOrderId { get; set; }
}

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


public class CustomMealDto : CustomMealSimplifyDto
{
    public virtual ICollection<CustomMealOptionDto>? SelectedOptions { get; set; }

}

public class CustomMealOptionDto : IDto
{
    public DefaultIdType OptionId { get; set; }
    public virtual MealCustomizationOptionDto? Option { get; set; }

    // You can add quantity if options can be added multiple times
    public int Quantity { get; set; } = 1;
}



public class MealCustomizationOptionDto : IDto
{

    public DefaultIdType Id { get; set; }


    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? ImageUrl { get; set; }
    public decimal PriceAdjustment { get; set; } // Can be positive or negative

    public MealCustomizationGroupSimplifyDto? Group { get; set; }
}

public class MealCustomizationGroupSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? ImageUrl { get; set; }
}
