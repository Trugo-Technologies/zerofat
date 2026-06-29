using ZeroFat.Application.Common.Interfaces;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.MealRatings;

public class MealRatingSummaryDto : IDto
{
    public DefaultIdType? Id { get; set; }
    public MealRatingValue? Rating { get; set; }
    public string? RatingLabel { get; set; }
    public List<string> ImprovementTags { get; set; } = [];
    public string? Comment { get; set; }
}

public class MealRatingDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType ClientId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerImageUrl { get; set; }
    public DefaultIdType DailyMealSelectionId { get; set; }
    public DefaultIdType? MealId { get; set; }
    public string? DishName { get; set; }
    public MealRatingValue Rating { get; set; }
    public string RatingLabel { get; set; } = string.Empty;
    public List<string> ImprovementTags { get; set; } = [];
    public string? AdditionalComment { get; set; }
    public string? Feedback { get; set; }
    public DateOnly MealDate { get; set; }
    public DateTime SubmittedOn { get; set; }
    public string? AdminReply { get; set; }
    public DateTime? AdminRepliedOn { get; set; }
    public bool CanReply { get; set; }
}

public class MealRatingFilterDto
{
    public string? Search { get; set; }
    public MealRatingValue? Rating { get; set; }
    public DefaultIdType? ClientId { get; set; }
    public DefaultIdType? MealId { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}
