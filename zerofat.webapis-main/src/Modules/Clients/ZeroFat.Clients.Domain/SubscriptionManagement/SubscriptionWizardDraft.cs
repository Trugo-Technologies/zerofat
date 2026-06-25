using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Domain.SubscriptionManagement;

/// <summary>
/// Persisted admin wizard state (draft until finalize).
/// Table: Client.SubscriptionWizardDrafts — migration addSubscriptionWizardDraft.
/// JSON columns: MealTypeSelections, AddOnItems, SelectedDeliveryDates.
/// </summary>
public class SubscriptionWizardDraft : AuditableEntity
{
    public DefaultIdType ClientId { get; set; }
    public DefaultIdType? CreatedByAdminId { get; set; }

    public SubscriptionWizardMode WizardMode { get; set; }
    public SubscriptionWizardStep CurrentStep { get; set; }
    public SubscriptionRenewalStrategy? RenewalStrategy { get; set; }
    public SubscriptionDraftStatus Status { get; set; } = SubscriptionDraftStatus.Draft;
    public DateTime ExpiresAt { get; set; }

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

    public DefaultIdType? FinalizedClientSubscriptionId { get; set; }
}

public class WizardMealTypeSelection
{
    public DefaultIdType MealTypeId { get; set; }
    public string? Name { get; set; }
    public int QuantityPerDay { get; set; }
}
