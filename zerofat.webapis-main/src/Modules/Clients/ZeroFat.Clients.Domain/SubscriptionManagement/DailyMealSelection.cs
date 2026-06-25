using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.ClientPortal.Domain.SubscriptionManagement;

[Index(nameof(Date))]
public class DailyMealSelection : AuditableEntity
{
    public DateOnly Date { get; set; }

    // Meal type classification
    public MealSelectionType MealSelectionType { get; set; }
    [DefaultValue(1)]
    public int Qty { get; set; } = 1;

    // Single meal reference (can be default, add-on, or custom meal details)
    public DefaultIdType? MealId { get; set; }
    public virtual Meal? Meal { get; set; }

    public DefaultIdType? CustomMealId { get; set; }

    // Pricing information
    public decimal BasePrice { get; set; }  // Original subscription price
    public decimal? AdjustedPrice { get; set; } // Final price after modifications
    public string? PriceAdjustmentReason { get; set; }

    // Nutritional information (reflects final meal)
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

    // Relationships
    public virtual DailySelection? DailySelection { get; set; } // The daily menu for the date
    public DefaultIdType? MealTypeId { get; set; } // Foreign key to the client
    public DefaultIdType? MealPlanId { get; set; } // Foreign key to the client
    public DefaultIdType DailySelectionId { get; set; }
    public DefaultIdType ClientSubscriptionId { get; set; }

    public DefaultIdType ClientId { get; set; }
}


