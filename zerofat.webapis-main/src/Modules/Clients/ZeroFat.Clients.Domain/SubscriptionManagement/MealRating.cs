using ZeroFat.Domain.Common.Contracts;

namespace ZeroFat.ClientPortal.Domain.SubscriptionManagement;

public enum MealRatingValue
{
    NotGood,
    Okay,
    Awesome
}

public enum MealRatingImprovementTag
{
    Flavor,
    PortionSize,
    Texture,
    FoodQuality,
    Packaging,
    Delivery,
    Other
}

/// <summary>
/// Client feedback for a delivered daily meal selection.
/// </summary>
public class MealRating : AuditableEntity, IAggregateRoot
{
    public DefaultIdType ClientId { get; set; }
    public virtual ClientManagement.Client? Client { get; set; }

    public DefaultIdType DailyMealSelectionId { get; set; }
    public virtual DailyMealSelection? DailyMealSelection { get; set; }

    public DefaultIdType? MealId { get; set; }
    public string? MealName { get; set; }
    public DateTime MealDate { get; set; }

    public MealRatingValue Rating { get; set; }
    public List<MealRatingImprovementTag> ImprovementTags { get; set; } = [];
    public string? Comment { get; set; }

    public string? AdminReply { get; set; }
    public DateTime? AdminRepliedOn { get; set; }
    public string? AdminRepliedByName { get; set; }
}
