using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Core.SubscriptionPlans;

public class SubscriptionPlanSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? TitleEn { get; set; }
    public string? TitleAr { get; set; }
    public int MinimumOrderValue { get; set; }
    public bool? PercentageDiscount { get; set; }
    public bool IsActive { get; set; }
    public double? DiscountAmount { get; set; }
    public string? PaymobId { get; set; }
    public ApplicationModule? SubscriptionModule { get; set; }
    public SubscriptionType SubscriptionType { get; set; }
}

public class SubscriptionPlanDetailsDto : SubscriptionPlanSimplifyDto
{
    public string? PaymobId { get; set; }

}
