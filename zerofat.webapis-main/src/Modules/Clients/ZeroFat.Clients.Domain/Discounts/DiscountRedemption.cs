namespace ZeroFat.ClientPortal.Domain.Discounts;

public class DiscountRedemption : Entity
{
    public DefaultIdType? DiscountRuleId { get; set; }
    public DefaultIdType? ClientId { get; set; }
    public int TimesUsed { get; set; }

    public DateTime LastUsedAt { get; set; }

    public virtual DiscountRule? DiscountRule { get; set; }
}
