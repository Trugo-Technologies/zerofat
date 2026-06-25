using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Domain.Catalog;

namespace ZeroFat.GymUp.Domain.Creator;
public class Plan : ActivationEntity, IAggregateRoot, IImageEntity  
{
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }

    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }

    public string? GetOriginalImageUrl() => AvatarImageUrl;
    public string? GetThumbnailUrl() => AvatarImageThumbnailUrl;
    public string? GetOptimizedUrl() => AvatarImageOptimzeUrl;

    public void SetThumbnailUrl(string url) => AvatarImageThumbnailUrl = url;
    public void SetOptimizedUrl(string url) => AvatarImageOptimzeUrl = url;


    public string? ProfileMediaUrl { get; set; }
    public int DaysPerWeek { get; set; }
    public int? WeeklyRepetition { get; set; }

    public DefaultIdType? TrainerId { get; set; }
    public DefaultIdType? PlanGoalId { get; set; }
    public DefaultIdType? EquipmentCategoryId { get; set; }


    public GymEnvironment? Environment { get; set; }
    public Level Level { get; set; }
    public string? OverviewAr { get; set; }
    public string? OverviewEn { get; set; }
    public string? PlanConclusionEn { get; set; }
    public string? PlanConclusionAr { get; set; }

    public virtual Trainer? Trainer { get; set; }
    public virtual EquipmentCategory? EquipmentCategory { get; set; }
    public virtual PlanGoal? PlanGoal { get; set; }

    public ICollection<PlanSchedule> Schedules { get; set; }
    public ICollection<PlanReview> PlanReviews { get; set; }
    public ICollection<PlanWishlist> PlanWishlists { get; set; }
    public List<int> RestDays { get; set; }
    public Plan()
    {
        Schedules = [];
        PlanReviews = [];
        PlanWishlists = [];
        RestDays = [];
    }
}
