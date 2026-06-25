using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.GymUp.Application.Catalog.WorkoutTypes;

public class WorkoutTypeSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; }
}

public class WorkoutTypeRawDto : WorkoutTypeSimplifyDto
{
}

public class WorkoutTypeAuditableDto : WorkoutTypeRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class WorkoutTypeDto : WorkoutTypeAuditableDto
{

}

public class WorkoutTypeDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? IconUrl { get; set; }
}
