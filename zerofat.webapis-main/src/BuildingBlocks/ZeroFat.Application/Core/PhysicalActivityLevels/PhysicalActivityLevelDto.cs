using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Core.PhysicalActivityLevels;


public class PhysicalActivityLevelSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public string? ImageOptimzeUrl { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? ExampleEn { get; set; }
    public string? ExampleAr { get; set; }
    public double ActivityValue { get; set; }
    public bool IsActive { get; set; }
}

public class PhysicalActivityLevelRawDto : PhysicalActivityLevelSimplifyDto
{
}

public class PhysicalActivityLevelAuditableDto : PhysicalActivityLevelRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class PhysicalActivityLevelDto : PhysicalActivityLevelAuditableDto
{

}

public class PhysicalActivityLevelDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public string? ImageOptimzeUrl { get; set; }

    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? ExampleEn { get; set; }
    public string? ExampleAr { get; set; }
    public double ActivityValue { get; set; }

}
