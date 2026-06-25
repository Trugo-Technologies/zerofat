using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Domain.Discounts;

public class DiscountRule : ActivationEntity
{
    public string Code { get; set; } = null!;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string? StripeId { get; set; }

    public long? PercentOff { get; set; }
    public long? AmountOff { get; set; }

    public DiscountDuration Duration { get; set; } = DiscountDuration.Once;
    public int DurationInMonths { get; set; } // Only used if Repeating

    public DateOnly? StartDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }

    public int MaxRedemptions { get; set; }
    public int RedemptionsUsed { get; set; }
    public int MaxRedemptionsPerClient { get; set; } = 1;

    // 🆕 Additional properties
    public bool FirstTimeClientsOnly { get; set; }      // Limit to new clients
}
