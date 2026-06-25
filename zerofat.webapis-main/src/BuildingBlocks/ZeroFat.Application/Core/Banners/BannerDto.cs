using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;

namespace ZeroFat.Application.Core.Banners;

public class BannerSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? TitleEn { get; set; }
    public string? TitleAr { get; set; }
    public int Index { get; set; }

    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public string? ImageOptimzeUrl { get; set; }
    public bool IsActive { get; set; }
}

public class BannerRawDto : BannerSimplifyDto
{
}

public class BannerAuditableDto : BannerRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class BannerDto : BannerAuditableDto
{

}

public class BannerDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? TitleEn { get; set; }
    public string? TitleAr { get; set; }
    public int Index { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public string? ImageOptimzeUrl { get; set; }
}
