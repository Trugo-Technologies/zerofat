using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.Contracts;

/// <summary>Input for <see cref="ISubscriptionPricingService.CalculateAsync"/>.</summary>
public class SubscriptionMealTypePricingInput
{
    public DefaultIdType MealTypeId { get; set; }
    public int QuantityPerDay { get; set; }
    public decimal Price { get; set; }
    public decimal AverageCalories { get; set; }
    public string? MealTypeNameEn { get; set; }
    public string? MealTypeNameAr { get; set; }
}

public class SubscriptionPricingInput
{
    public SubscriptionType SubscriptionType { get; set; }
    public List<DayOfWeek> SelectedDeliveryDays { get; set; } = [];
    public List<SubscriptionMealTypePricingInput> MealTypeSelections { get; set; } = [];
    public List<SubscriptionWizardAddOnItem> AddOnItems { get; set; } = [];
    public string? PromoCode { get; set; }
    public decimal ManualDiscountAed { get; set; }
    public DateOnly StartDate { get; set; }
}

/// <summary>Quote/invoice breakdown returned by pricing service (wizard preview + finalize).</summary>
public class SubscriptionPricingResult
{
    public decimal PlanAmount { get; set; }
    public decimal AddOnAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TotalCost { get; set; }
    public decimal AverageCalories { get; set; }
    public DateOnly EndDate { get; set; }
    public StripeCouponDto? AppliedCoupon { get; set; }
}

/// <summary>
/// Central pricing for subscriptions (wizard + client subscribe/renew).
/// Implementation: SubscriptionPricingService in ClientPortal.Infrastructure.
/// </summary>
public interface ISubscriptionPricingService : ITransientService
{
    Task<SubscriptionPricingResult> CalculateAsync(SubscriptionPricingInput input, CancellationToken cancellationToken = default);
    (DateOnly EndDate, int PeriodMultiplier) GetDurationRules(SubscriptionType subscriptionType, DateOnly startDate);
}
