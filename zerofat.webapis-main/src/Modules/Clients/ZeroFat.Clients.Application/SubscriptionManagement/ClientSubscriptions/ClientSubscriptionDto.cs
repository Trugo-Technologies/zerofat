using ZeroFat.Application.Common.Interfaces;
using ZeroFat.ClientPortal.Application.ClientManagement;
using ZeroFat.ClientPortal.Application.ClientManagement.Clients;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;

public class ClientSubscriptionSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }

    public SubscriptionType SubscriptionType { get; set; } // Type of subscription (e.g., One Week, One Month, Three Months)
    public DateOnly StartDate { get; set; } // Start date of the subscription
    public DateOnly EndDate { get; set; } // End date of the subscription
                                          // Delivery Preferences

    public List<DayOfWeek> SelectedDeliveryDays { get; set; } = []; // Days for delivery
    public PreferredMealTime PreferredDeliveryTime { get; set; }

    public decimal TotalCost { get; set; } // Total cost of the subscription
    public decimal AverageCalories { get; set; }
    public string? PaymentQuickLink { get; set; }

    public PaymentStatus PaymentStatus { get; set; } // Payment status of the subscription (e.g., "Paid", "Pending")
    public SubscriptionStatus SubscriptionStatus { get; set; } // Status of the subscription (e.g., Active, Cancelled)

    public DefaultIdType? ClientLocationId { get; set; }

    public DateTime? LastSyncDate { get; set; }
    public DateTime? CancelAt { get; set; }
    public DateTime? PaymentDate { get; set; }

    // Recurrence & Lifecycle
    public bool IsAutoRenewalEnabled { get; set; }
    public int RenewalCount { get; set; }
    public DateOnly? NextRenewalDate { get; set; }
    public DateTime? LastStatusUpdate { get; set; }

    public DefaultIdType MealPlanId { get; set; } // Foreign key to the client
    public DefaultIdType ClientId { get; set; } // Foreign key to the client
}

public class ClientSubscriptionRawDto : ClientSubscriptionSimplifyDto
{
}

public class ClientSubscriptionAuditableDto : ClientSubscriptionRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class ClientSubscriptionDto : ClientSubscriptionAuditableDto
{
    public ClientSimplifyDto? Client { get; set; }
    public MealPlanDto? MealPlan { get; set; }
    public List<MealTypeSelectionDto> SelectedMealTypes { get; set; } = [];
    public ClientLocationSimplifyDto? ClientLocation { get; set; }
}

public class ClientSubscriptionDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }

    public SubscriptionType SubscriptionType { get; set; } // Type of subscription (e.g., One Week, One Month, Three Months)
    public DateOnly StartDate { get; set; } // Start date of the subscription
    public DateOnly EndDate { get; set; } // End date of the subscription
    public List<DayOfWeek> SelectedDeliveryDays { get; set; } = []; // Days for delivery
    public PreferredMealTime PreferredDeliveryTime { get; set; }

    public decimal TotalCost { get; set; } // Total cost of the subscription
    public decimal AverageCalories { get; set; }

    public PaymentStatus PaymentStatus { get; set; } // Payment status of the subscription (e.g., "Paid", "Pending")
    public SubscriptionStatus SubscriptionStatus { get; set; } // Status of the subscription (e.g., Active, Cancelled)

    public DefaultIdType? ClientLocationId { get; set; }

    public DateTime? LastSyncDate { get; set; }
    public DateTime? CancelAt { get; set; }
    public DateTime? PaymentDate { get; set; }

    // Recurrence & Lifecycle
    public bool IsAutoRenewalEnabled { get; set; }
    public int RenewalCount { get; set; }
    public DateOnly? NextRenewalDate { get; set; }
    public DateTime? LastStatusUpdate { get; set; }

    public DefaultIdType MealPlanId { get; set; } // Foreign key to the client
    public DefaultIdType ClientId { get; set; } // Foreign key to the client

    public ClientSimplifyDto? Client { get; set; }
    public ClientLocationSimplifyDto? ClientLocation { get; set; }
    public MealPlanDto? MealPlan { get; set; }
    public List<MealTypeSelectionDto> SelectedMealTypes { get; set; } = [];
}


public class MealPlanDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? Code { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? CarbPercentage { get; set; }
    public decimal? ProteinPercentage { get; set; }
    public decimal? FatPercentage { get; set; }
    public bool IsActive { get; set; }
}

public class MealPlanMealTypeDto : IDto
{
    public decimal? AverageCalories { get; set; }
    public decimal? Price { get; set; }
    public MealTypeDto? MealType { get; set; }
}

public class MealTypeDto : IDto
{
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
}


public class MealTypeSelectionDto : IDto
{
    public int QuantityPerDay { get; set; } // The number of meals of this type the client has selected (e.g., 3 breakfasts)

    public string? Notes { get; set; } // Optional notes for this meal type selection (e.g., "Extra protein")

    public MealTypeDto? MealType { get; set; } // The meal plan type (e.g., Keto, High Protein)
}
