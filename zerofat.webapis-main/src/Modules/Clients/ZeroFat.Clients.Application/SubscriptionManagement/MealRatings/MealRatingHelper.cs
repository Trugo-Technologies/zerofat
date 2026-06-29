using Ardalis.Specification;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.MealRatings;

internal static class MealRatingHelper
{
    public static string GetAppRatingLabel(MealRatingValue rating) => rating switch
    {
        MealRatingValue.NotGood => "Not good",
        MealRatingValue.Okay => "Okay",
        MealRatingValue.Awesome => "Awesome",
        _ => rating.ToString()
    };

    public static string GetAdminRatingLabel(MealRatingValue rating) => rating switch
    {
        MealRatingValue.NotGood => "Not Good",
        MealRatingValue.Okay => "Good",
        MealRatingValue.Awesome => "Awesome",
        _ => rating.ToString()
    };

    public static string GetImprovementTagLabel(MealRatingImprovementTag tag) => tag switch
    {
        MealRatingImprovementTag.Flavor => "Flavor",
        MealRatingImprovementTag.PortionSize => "Portion Size",
        MealRatingImprovementTag.Texture => "Texture",
        MealRatingImprovementTag.FoodQuality => "Food Quality",
        MealRatingImprovementTag.Packaging => "Packaging",
        MealRatingImprovementTag.Delivery => "Delivery",
        MealRatingImprovementTag.Other => "Other",
        _ => tag.ToString()
    };

    public static List<string> MapImprovementTags(IEnumerable<MealRatingImprovementTag> tags)
        => tags.Select(GetImprovementTagLabel).ToList();

    public static string? BuildAdditionalComment(IEnumerable<MealRatingImprovementTag> tags)
    {
        var labels = MapImprovementTags(tags);
        return labels.Count == 0 ? null : string.Join(", ", labels);
    }

    public static MealRatingSummaryDto ToSummaryDto(MealRating entity) => new()
    {
        Id = entity.Id,
        Rating = entity.Rating,
        RatingLabel = GetAppRatingLabel(entity.Rating),
        ImprovementTags = MapImprovementTags(entity.ImprovementTags),
        Comment = entity.Comment
    };

    public static MealRatingDto ToAdminDto(MealRating entity) => new()
    {
        Id = entity.Id,
        ClientId = entity.ClientId,
        CustomerName = entity.Client?.FullName,
        CustomerImageUrl = entity.Client?.ImageUrl,
        DailyMealSelectionId = entity.DailyMealSelectionId,
        MealId = entity.MealId,
        DishName = entity.MealName,
        Rating = entity.Rating,
        RatingLabel = GetAdminRatingLabel(entity.Rating),
        ImprovementTags = MapImprovementTags(entity.ImprovementTags),
        AdditionalComment = BuildAdditionalComment(entity.ImprovementTags),
        Feedback = entity.Comment,
        MealDate = entity.MealDate,
        SubmittedOn = entity.CreatedOn,
        AdminReply = entity.AdminReply,
        AdminRepliedOn = entity.AdminRepliedOn,
        CanReply = string.IsNullOrWhiteSpace(entity.AdminReply)
            && (entity.Rating != MealRatingValue.Awesome
                || !string.IsNullOrWhiteSpace(entity.Comment)
                || entity.ImprovementTags.Count > 0)
    };

    public static void ApplyFilters(
        ISpecificationBuilder<MealRating> query,
        MealRatingFilterDto filters)
    {
        if (filters.Rating.HasValue)
        {
            query.Where(x => x.Rating == filters.Rating.Value);
        }

        if (filters.ClientId.HasValue)
        {
            query.Where(x => x.ClientId == filters.ClientId.Value);
        }

        if (filters.MealId.HasValue)
        {
            query.Where(x => x.MealId == filters.MealId.Value);
        }

        if (filters.DateFrom.HasValue)
        {
            query.Where(x => x.MealDate >= filters.DateFrom.Value);
        }

        if (filters.DateTo.HasValue)
        {
            query.Where(x => x.MealDate <= filters.DateTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var term = filters.Search.Trim().ToLowerInvariant();
            query.Where(x =>
                (x.MealName != null && x.MealName.ToLower().Contains(term))
                || (x.Comment != null && x.Comment.ToLower().Contains(term))
                || (x.Client != null && x.Client.FullName != null && x.Client.FullName.ToLower().Contains(term))
                || (x.AdminReply != null && x.AdminReply.ToLower().Contains(term)));
        }
    }
}
