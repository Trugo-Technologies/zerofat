using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.Discounts.DiscountRules;

public class DiscountRuleSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string Code { get; set; } = null!;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string? StripeId { get; set; } 

    public long? PercentOff { get; set; }
    public long? AmountOff { get; set; }

    public DiscountDuration Duration { get; set; }
    public int DurationInMonths { get; set; } // Only used if Repeating

    public DateOnly? StartDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }

    public int MaxRedemptions { get; set; }
    public int RedemptionsUsed { get; set; }
    public int MaxRedemptionsPerClient { get; set; } = 1;

    // 🆕 Additional properties
    public bool FirstTimeClientsOnly { get; set; }      // Limit to new clients
    public bool IsActive { get; set; }      // Limit to new clients


}

public class DiscountRuleRawDto : DiscountRuleSimplifyDto
{

}

public class DiscountRuleAuditableDto : DiscountRuleRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class DiscountRuleDto : DiscountRuleAuditableDto
{
}

public class DiscountRuleDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string Code { get; set; } = null!;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string? StripeId { get; set; }

    public decimal? PercentOff { get; set; }
    public decimal? AmountOff { get; set; }

    public DiscountDuration Duration { get; set; }
    public int DurationInMonths { get; set; } // Only used if Repeating

    public DateOnly? StartDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }

    public int MaxRedemptions { get; set; }
    public int RedemptionsUsed { get; set; }
    public int MaxRedemptionsPerClient { get; set; } = 1;

    // 🆕 Additional properties
    public bool FirstTimeClientsOnly { get; set; }      // Limit to new clients
}


