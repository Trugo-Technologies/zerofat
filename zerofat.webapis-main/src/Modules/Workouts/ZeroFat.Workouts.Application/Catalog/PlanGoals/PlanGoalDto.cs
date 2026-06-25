using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.GymUp.Application.Catalog.PlanGoals;

public class PlanGoalSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public string? ImageOptimzeUrl { get; set; }
    public bool IsActive { get; set; }
}

public class PlanGoalRawDto : PlanGoalSimplifyDto
{
}

public class PlanGoalAuditableDto : PlanGoalRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class PlanGoalDto : PlanGoalAuditableDto
{

}

public class PlanGoalDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public string? ImageOptimzeUrl { get; set; }
}

