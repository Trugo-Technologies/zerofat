using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Application.Catalog.EquipmentCategories;
using ZeroFat.GymUp.Application.Catalog.PlanGoals;
using ZeroFat.GymUp.Application.Creator.PlanReviews;
using ZeroFat.GymUp.Application.Creator.PlanSchedules;
using ZeroFat.GymUp.Application.Creator.Trainers;

namespace ZeroFat.GymUp.Application.Creator.Plans;
public class PlanMobileDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }
    public string? ProfileMediaUrl { get; set; }
    public int DaysPerWeek { get; set; }
    public int? WeeklyRepetition { get; set; }
    public GymEnvironment? Environment { get; set; }
    public Level Level { get; set; }
    public double? TotalRate { get; set; }
    public bool? IsWishlist { get; set; }
}

public class PlanSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }
    public string? ProfileMediaUrl { get; set; }
    public int DaysPerWeek { get; set; }
    public GymEnvironment? Environment { get; set; }
    public Level Level { get; set; }
    public bool IsActive { get; set; }
}

public class PlanRawDto : PlanSimplifyDto
{
    public int? WeeklyRepetition { get; set; }
    public DefaultIdType? TrainerId { get; set; }
    public DefaultIdType PlanGoalId { get; set; }

}

public class PlanAuditableDto : PlanRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class PlanDto : PlanAuditableDto
{
    public List<int> RestDays { get; set; } = [];
    public TrainerSimplifyDto? Trainer { get; set; }
    public PlanGoalSimplifyDto? PlanGoal { get; set; }
    public EquipmentCategorySimplifyDto? EquipmentCategory { get; set; }
}

public class PlanDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }
    public int DaysPerWeek { get; set; }
    public Level Level { get; set; }
    public GymEnvironment? Environment { get; set; }
    public string? ProfileMediaUrl { get; set; }
    public int? WeeklyRepetition { get; set; }
    public DefaultIdType? TrainerId { get; set; }
    public DefaultIdType PlanGoalId { get; set; }
    public string? OverviewAr { get; set; }
    public string? OverviewEn { get; set; }
    public string? PlanConclusionEn { get; set; }
    public string? PlanConclusionAr { get; set; }
    public List<int> RestDays { get; set; } = [];
    public TrainerSimplifyDto? Trainer { get; set; }
    public PlanGoalSimplifyDto? PlanGoal { get; set; }
}

public class PlanMobileDetailsDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public int DaysPerWeek { get; set; }
    public Level Level { get; set; }
    public GymEnvironment? Environment { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }
    public string? ProfileMediaUrl { get; set; }
    public int? WeeklyRepetition { get; set; }
    public string? OverviewAr { get; set; }
    public string? OverviewEn { get; set; }
    public string? PlanConclusionEn { get; set; }
    public string? PlanConclusionAr { get; set; }
    public double? TotalRate { get; set; }
    public double? EffectivenessRate { get; set; }
    public double? EasyToUseRate { get; set; }
    public double? EnjoyabilityRate { get; set; }
    public bool? IsWishlist { get; set; }
    public List<int> RestDays { get; set; } = [];
    public PlanGoalSimplifyDto? PlanGoal { get; set; }
    public EquipmentCategorySimplifyDto? EquipmentCategory { get; set; }
    public List<PlanScheduleSimplifyDto>? Schedules { get; set; }
    public List<PlanReviewSimplifyDto>? PlanReviews { get; set; }
}
