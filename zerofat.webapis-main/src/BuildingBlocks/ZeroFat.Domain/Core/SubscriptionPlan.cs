using ZeroFat.Domain.Core.Common;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Domain.Core;

public class SubscriptionPlan : ActivationEntity, ICoreAggregateRoot
{
    public SubscriptionType SubscriptionType { get; set; }
    public string? PaymobId { get; set; }
    public string? StripeId { get; set; }
    public bool? PercentageDiscount { get; set; } // if null there is no discount
    public double? DiscountAmount { get; set; }
    public ApplicationModule? SubscriptionModule { get; set; }
}

