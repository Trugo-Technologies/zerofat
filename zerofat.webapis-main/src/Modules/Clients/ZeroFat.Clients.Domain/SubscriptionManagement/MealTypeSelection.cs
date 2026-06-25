using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Domain.SubscriptionManagement;

/// <summary>
/// Represents the number of a specific meal type selected by the client as part of their subscription.
/// </summary>
public class MealTypeSelection : Entity
{
    public int QuantityPerDay { get; set; } // The number of meals of this type the client has selected (e.g., 3 breakfasts)
    public decimal Price { get; set; } // The number of meals of this type the client has selected (e.g., 3 breakfasts)
    public string? MealTypeNameEn { get; set; } // The number of meals of this type the client has selected (e.g., 3 breakfasts)
    public string? MealTypeNameAr { get; set; } // The number of meals of this type the client has selected (e.g., 3 breakfasts)

    public string? Notes { get; set; } // Optional notes for this meal type selection (e.g., "Extra protein")

    public DefaultIdType MealTypeId { get; set; } // Foreign key to the client
    public virtual MealType? MealType { get; set; } // The meal plan type (e.g., Keto, High Protein)

    public DefaultIdType ClientSubscriptionId { get; set; } // Foreign key to the client
    public virtual ClientSubscription? ClientSubscription { get; set; } // Foreign key to the client
}
