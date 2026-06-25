using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Core.Advertisements;

public class AdvertisementSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? TitleEn { get; set; }
    public string? TitleAr { get; set; }
    public int Index { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? WelcomeMsgAr { get; set; }
    public string? WelcomeMsgEn { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public string? ImageOptimzeUrl { get; set; }
    public bool IsActive { get; set; }
}

public class AdvertisementRawDto : AdvertisementSimplifyDto
{
}

public class AdvertisementAuditableDto : AdvertisementRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class AdvertisementDto : AdvertisementAuditableDto
{

}

public class AdvertisementDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? TitleEn { get; set; }
    public string? TitleAr { get; set; }
    public int Index { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? WelcomeMsgAr { get; set; }
    public string? WelcomeMsgEn { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public string? ImageOptimzeUrl { get; set; }
}
