using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

internal static class ClientDeliveryCalendarPricingHelper
{
    public static SubscriptionPricingInput BuildPricingInput(ClientSubscription subscription)
    {
        return new SubscriptionPricingInput
        {
            SubscriptionType = subscription.SubscriptionType,
            SelectedDeliveryDays = subscription.SelectedDeliveryDays,
            StartDate = subscription.StartDate,
            PromoCode = subscription.PromoCode,
            ManualDiscountAed = subscription.ManualDiscountAed ?? 0,
            AddOnItems = subscription.AddOnItems,
            MealTypeSelections = subscription.SelectedMealTypes.Select(x => new SubscriptionMealTypePricingInput
            {
                MealTypeId = x.MealTypeId,
                QuantityPerDay = x.QuantityPerDay,
                Price = x.Price,
                AverageCalories = 0,
                MealTypeNameEn = x.MealTypeNameEn,
                MealTypeNameAr = x.MealTypeNameAr
            }).ToList()
        };
    }

    public static async Task<List<SubscriptionMealTypePricingInput>> BuildMealTypePricingInputsAsync(
        DefaultIdType mealPlanId,
        IEnumerable<ClientDeliveryMealPlanSlotDto> mealSlots,
        IReadRepository<MealPlanMealType> mealPlanMealTypeRepo,
        CancellationToken cancellationToken)
    {
        var inputs = new List<SubscriptionMealTypePricingInput>();

        foreach (var slot in mealSlots.Where(x => x.Enabled && x.QuantityPerDay > 0))
        {
            var mealPlanMealType = await mealPlanMealTypeRepo.FirstOrDefaultAsync(
                new ExpressionSpecificationProjecting<MealPlanMealType, MealPlanMealTypeDto>(
                    x => x.MealPlanId == mealPlanId && x.MealTypeId == slot.MealTypeId),
                cancellationToken);

            inputs.Add(new SubscriptionMealTypePricingInput
            {
                MealTypeId = slot.MealTypeId,
                QuantityPerDay = slot.QuantityPerDay,
                Price = mealPlanMealType?.Price ?? 0,
                AverageCalories = mealPlanMealType?.AverageCalories ?? 0,
                MealTypeNameEn = mealPlanMealType?.MealType?.NameEn,
                MealTypeNameAr = mealPlanMealType?.MealType?.NameAr
            });
        }

        return inputs;
    }

    public static decimal CalculateProratedUpgradeAmount(
        ClientSubscription subscription,
        decimal oldTotalCost,
        decimal newTotalCost)
    {
        var periodDiff = newTotalCost - oldTotalCost;
        if (periodDiff <= 0)
        {
            return 0;
        }

        var totalDays = Math.Max(1, subscription.EndDate.DayNumber - subscription.StartDate.DayNumber + 1);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var effectiveStart = today > subscription.StartDate ? today : subscription.StartDate;
        var remainingDays = Math.Max(0, subscription.EndDate.DayNumber - effectiveStart.DayNumber + 1);

        return Math.Round(periodDiff * remainingDays / totalDays, 2, MidpointRounding.AwayFromZero);
    }
}
