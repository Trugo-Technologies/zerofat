using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Shared;

public interface IClientService : ITransientService
{
    Task<ClientSharedDto?> GetClientById(Guid clientId);
    Task<List<DefaultIdType>?> GetClientAllergicIdsByClientId(DefaultIdType clientId);
    Task<ClientSubscriptionSharedDto?> GetClientSubscriptionById(Guid clientSubscriptionId);
    Task UpdateClientOrEmail(DefaultIdType clientId, string? mail, string? phoneNumber);
    Task<ClientPaymentMethodShareDto?> GetClientDefaultPaymentMethod(DefaultIdType clientId);
    Task UpdateClientStatisticesFromWorkout(DateOnly date, DefaultIdType clientId, double calories, string nameEn);

    Task DeactivateClientAsync(DefaultIdType clientId);

    Task<bool> GetClientStatusByClientId(DefaultIdType clientId);
}

public class ClientSharedDto : IDto
{
    public string? FullName { get; set; }

    public string? Email { get; set; }
    public Guid? ClientSubscriptionId { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; }

    public Gender? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ImageUrl { get; set; }

    public double? HeightInCM { get; set; }
    public double? WeightInKG { get; set; }
    public double? CurrentWeightInKG { get; set; }

    public double? TargetWeightInKG { get; set; }
    public DietitianGoal DietitianGoal { get; set; }
    public double ActivityValue { get; set; }
    public DateOnly? NewGoalStart { get; set; }

    public double BMI { get; set; }
    public double BMR { get; set; }
    public double BodyFat { get; set; }
    public double TDEE { get; set; }
    public int TimeToReachGoalInDays { get; set; }
    public double NeededCaloriesToReachGoal { get; set; }
    public List<DefaultIdType> ClientAllergicIds { get; set; } = new List<DefaultIdType>();
}

public class ClientSubscriptionSharedDto : IDto
{
    public DefaultIdType Id { get; set; }

    public SubscriptionType SubscriptionType { get; set; } // Type of subscription (e.g., One Week, One Month, Three Months)
    public DateOnly StartDate { get; set; } // Start date of the subscription
    public DateOnly EndDate { get; set; } // End date of the subscription
    public List<DayOfWeek> SelectedDeliveryDays { get; set; } = []; // Days for delivery
    public PreferredMealTime PreferredDeliveryTime { get; set; }

    public bool HasCutlery { get; set; }

    public decimal AverageCalories { get; set; }

    public PaymentStatus PaymentStatus { get; set; } // Payment status of the subscription (e.g., "Paid", "Pending")
    public SubscriptionStatus SubscriptionStatus { get; set; } // Status of the subscription (e.g., Active, Cancelled)

    public DefaultIdType? ClientLocationId { get; set; }
    public DefaultIdType MealPlanId { get; set; }
    public MealPlanShareDto? MealPlan { get; set; }
    public string? PaymentQuickLink { get; set; }

    public DateTime? LastSyncDate { get; set; }
    public DateTime? CancelAt { get; set; }
    public DateTime? PaymentDate { get; set; }

    // Recurrence & Lifecycle
    public bool IsAutoRenewalEnabled { get; set; }
    public int RenewalCount { get; set; }
    public DateOnly? NextRenewalDate { get; set; }
    public DateTime? LastStatusUpdate { get; set; }
}

public class MealPlanShareDto : IDto
{
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? ImageUrl { get; set; }

    public DietitianGoal DefaultDietitianGoal { get; set; }

    public decimal? CarbPercentage { get; set; }
    public decimal? ProteinPercentage { get; set; }
    public decimal? FatPercentage { get; set; }
}


public class ClientPaymentMethodShareDto : IDto
{
    public DefaultIdType Id { get; set; }
    public bool IsDefault { get; set; }
    public string? StripeId { get; set; }
    public string? Type { get; set; }
    public string? CardBrand { get; set; }
    public int? CardExpMonth { get; set; }
    public string? CardFunding { get; set; }
    public string? CardLast4 { get; set; }
    public string? CardName { get; set; }
}
