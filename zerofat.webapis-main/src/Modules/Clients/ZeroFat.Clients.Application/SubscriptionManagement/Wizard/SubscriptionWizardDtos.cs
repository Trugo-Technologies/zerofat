using ZeroFat.Application.Common.Interfaces;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

public class SubscriptionWizardDraftDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType ClientId { get; set; }
    public SubscriptionWizardMode WizardMode { get; set; }
    public SubscriptionWizardStep CurrentStep { get; set; }
    public SubscriptionRenewalStrategy? RenewalStrategy { get; set; }
    public SubscriptionDraftStatus Status { get; set; }
    public bool HasActiveSubscription { get; set; }
    public DateOnly? ActiveSubscriptionEndDate { get; set; }
    public string? ActiveMealPlanName { get; set; }

    public DefaultIdType? MealPlanId { get; set; }
    public string? PlanVariant { get; set; }
    public int? CalorieTarget { get; set; }
    public int? ProteinTargetG { get; set; }
    public List<WizardMealTypeSelection> MealTypeSelections { get; set; } = [];
    public List<SubscriptionWizardAddOnItem> AddOnItems { get; set; } = [];

    public SubscriptionType? SubscriptionType { get; set; }
    public bool SkipSaturdays { get; set; }
    public bool SkipSundays { get; set; }
    public List<DateOnly> SelectedDeliveryDates { get; set; } = [];
    public List<DayOfWeek> SelectedDeliveryDays { get; set; } = [];
    public PreferredMealTime? PreferredDeliveryTime { get; set; }
    public DefaultIdType? ClientLocationId { get; set; }
    public string? PromoCode { get; set; }
    public decimal ManualDiscountAed { get; set; }

    public string? CustomerEmail { get; set; }
    public string? OptionalMessage { get; set; }
    public DateOnly? ScheduledStartDate { get; set; }
}

public class SubscriptionWizardPreviewDto : IDto
{
    public string? PlanName { get; set; }
    public string? PlanDuration { get; set; }
    public int MealsPerDay { get; set; }
    public string? DeliveryAddress { get; set; }
    public decimal PlanAmount { get; set; }
    public decimal AddOnAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal VatAmount { get; set; }
    public decimal Total { get; set; }
    public decimal AverageCalories { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}

public class WizardMealPlanOptionDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public decimal? MonthlyPrice { get; set; }
    public int MealTypesCount { get; set; }
    public decimal? AverageCalories { get; set; }
}

public class WizardAddOnOptionDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public decimal UnitPrice { get; set; }
    public SubscriptionAddOnCategory Category { get; set; }
}

public class ClientAccountAccessDto : IDto
{
    public DefaultIdType ClientId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool IsActive { get; set; }
    public double BMI { get; set; }
    public double BMR { get; set; }
    public double BodyFat { get; set; }
    public double TDEE { get; set; }
    public double? HeightInCM { get; set; }
    public double? WeightInKG { get; set; }
    public double? TargetWeightInKG { get; set; }
    public DietitianGoal DietitianGoal { get; set; }
    public int TimeToReachGoalInDays { get; set; }
    public double NeededCaloriesToReachGoal { get; set; }
    public List<string> Allergens { get; set; } = [];
    public List<ClientAccountAccessAddressDto> DeliveryAddresses { get; set; } = [];
    public ClientSubscriptionSummaryDto? Subscription { get; set; }
}

public class ClientAccountAccessAddressDto : IDto
{
    public DefaultIdType? Id { get; set; }
    public AddressType? Type { get; set; }
    public string? Area { get; set; }
    public string? Building { get; set; }
    public string? Flat { get; set; }
    public string? Street { get; set; }
}

public class ClientSubscriptionSummaryDto : IDto
{
    public DefaultIdType? SubscriptionId { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; }
    public string? ActiveMealPlanName { get; set; }
    public decimal AverageCalories { get; set; }
    public int MealsPerDay { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int RemainingDays { get; set; }
    public int TotalDeliveredMeals { get; set; }
}
