using System.ComponentModel.DataAnnotations.Schema;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Domain.SubscriptionManagement;

/// <summary>
/// Represents a client's subscription to a meal plan.
/// </summary>
public class ClientSubscription : AuditableEntity
{
    public ClientSubscription()
    {
        SelectedDeliveryDays = [];
        SelectedMealTypes = [];
        Payments = [];
    }

    // Identifiers
    public DefaultIdType ClientId { get; set; }
    [ForeignKey("ClientId")]
    public virtual Client? Client { get; set; }

    public DefaultIdType MealPlanId { get; set; }
    public virtual MealPlan? MealPlan { get; set; }

    public DefaultIdType? ClientLocationId { get; set; }


    public SubscriptionType SubscriptionType { get; set; } // Type of subscription (e.g., One Week, One Month, Three Months)
    public DateOnly StartDate { get; set; } // Start date of the subscription
    public DateOnly EndDate { get; set; } // End date of the subscription

    public virtual ICollection<MealTypeSelection> SelectedMealTypes { get; set; } // List of meal types selected by the client
    public virtual ICollection<Payment> Payments { get; set; } // Meals selected by the client

    // Delivery Preferences
    public List<DayOfWeek> SelectedDeliveryDays { get; set; } = []; // Days for delivery
    public PreferredMealTime PreferredDeliveryTime { get; set; }

    public decimal TotalCost { get; set; } // Total cost of the subscription
    public decimal AverageCalories { get; set; }
    public string? PaymentQuickLink { get; set; }

    public PaymentStatus PaymentStatus { get; set; } // Payment status of the subscription (e.g., "Paid", "Pending")
    public SubscriptionStatus SubscriptionStatus { get; set; } // Status of the subscription (e.g., Active, Cancelled)

    public string? PaymentOrderId { get; set; }
    public string? StripeSubscriptionId { get; set; }

    public DateTime? LastSyncDate { get; set; }
    public DateTime? CancelAt { get; set; }
    public DateTime? PaymentDate { get; set; }

    // Recurrence & Lifecycle
    public bool IsAutoRenewalEnabled { get; set; }
    public int RenewalCount { get; set; }
    public DateOnly? NextRenewalDate { get; set; }
    public DateTime? LastStatusUpdate { get; set; }
}

