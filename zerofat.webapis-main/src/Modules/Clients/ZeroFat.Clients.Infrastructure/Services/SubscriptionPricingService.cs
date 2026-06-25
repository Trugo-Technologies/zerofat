using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Infrastructure.Services;

/// <summary>
/// Shared subscription pricing used by:
/// - Admin wizard preview/finalize (ManageSubscriptions)
/// - Client self-subscribe (POST ClientSubscriptions/subscribe)
/// - Client renew (POST ClientSubscriptions/renew-subscribe)
///
/// Formula: (meal prices × qty × delivery days × period multiplier) − package/coupon/manual discounts + 5% VAT.
/// Durations: OneWeek, TwoWeeks, ThreeWeeks, OneMonth, TwoMonths, ThreeMonths.
/// </summary>
internal sealed class SubscriptionPricingService(
    IClientPortalSettingservice clientPortalSettingservice,
    IStripeService stripeService) : ISubscriptionPricingService
{
    private const decimal VatRate = 0.05m; // UAE VAT 5%

    /// <summary>Maps subscription type to end date and weekly period multiplier (4 weeks ≈ 1 month).</summary>
    public (DateOnly EndDate, int PeriodMultiplier) GetDurationRules(SubscriptionType subscriptionType, DateOnly startDate) =>
        subscriptionType switch
        {
            SubscriptionType.OneWeek => (startDate.AddDays(6), 1),
            SubscriptionType.TwoWeeks => (startDate.AddDays(13), 2),
            SubscriptionType.ThreeWeeks => (startDate.AddDays(20), 3),
            SubscriptionType.OneMonth => (startDate.AddDays(27), 4),
            SubscriptionType.TwoMonths => (startDate.AddDays(55), 8),
            SubscriptionType.ThreeMonths => (startDate.AddDays((28 * 3) - 1), 12),
            _ => (startDate.AddDays(6), 1)
        };

    public async Task<SubscriptionPricingResult> CalculateAsync(
        SubscriptionPricingInput input,
        CancellationToken cancellationToken = default)
    {
        var deliveryDayCount = Math.Max(input.SelectedDeliveryDays.Count, 1);
        decimal planAmount = 0;
        decimal averageCalories = 0;

        foreach (var selection in input.MealTypeSelections.Where(x => x.QuantityPerDay > 0))
        {
            planAmount += selection.QuantityPerDay * selection.Price;
            averageCalories += selection.QuantityPerDay * selection.AverageCalories;
        }

        planAmount *= deliveryDayCount;

        var (endDate, periodMultiplier) = GetDurationRules(input.SubscriptionType, input.StartDate);
        planAmount *= periodMultiplier;

        planAmount = await ApplyPackageDiscountAsync(input.SubscriptionType, planAmount, cancellationToken);

        var addOnAmount = input.AddOnItems.Sum(x => x.UnitPrice * x.Quantity);

        decimal discountAmount = input.ManualDiscountAed;
        StripeCouponDto? coupon = null;

        if (!string.IsNullOrWhiteSpace(input.PromoCode))
        {
            coupon = await stripeService.GetCouponByCodeAsync(input.PromoCode);
            if (coupon?.IsValid == true)
            {
                if (coupon.PercentOff.HasValue)
                {
                    discountAmount += planAmount * (coupon.PercentOff.Value / 100m);
                }
                else if (coupon.AmountOff.HasValue)
                {
                    discountAmount += coupon.AmountOff.Value / 100m;
                }
            }
        }

        var taxableAmount = Math.Max(0, planAmount + addOnAmount - discountAmount);
        var vatAmount = Math.Round(taxableAmount * VatRate, 2, MidpointRounding.AwayFromZero);
        var totalCost = taxableAmount + vatAmount;

        return new SubscriptionPricingResult
        {
            PlanAmount = Math.Round(planAmount, 2, MidpointRounding.AwayFromZero),
            AddOnAmount = Math.Round(addOnAmount, 2, MidpointRounding.AwayFromZero),
            DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero),
            VatAmount = vatAmount,
            TotalCost = Math.Round(totalCost, 2, MidpointRounding.AwayFromZero),
            AverageCalories = averageCalories,
            EndDate = endDate,
            AppliedCoupon = coupon
        };
    }

    private async Task<decimal> ApplyPackageDiscountAsync(
        SubscriptionType subscriptionType,
        decimal planAmount,
        CancellationToken cancellationToken)
    {
        if (subscriptionType is SubscriptionType.OneMonth or SubscriptionType.TwoMonths)
        {
            var monthlyDiscount = await clientPortalSettingservice.GetMonthlyPackageSubsciptionDiscount();
            if (monthlyDiscount > 0)
            {
                planAmount -= planAmount * monthlyDiscount / 100;
            }
        }

        if (subscriptionType is SubscriptionType.ThreeMonths)
        {
            var threeMonthlyDiscount = await clientPortalSettingservice.GetThreeMonthlyPackageSubsciptionDiscount();
            if (threeMonthlyDiscount > 0)
            {
                planAmount -= planAmount * threeMonthlyDiscount / 100;
            }
        }

        return planAmount;
    }
}
