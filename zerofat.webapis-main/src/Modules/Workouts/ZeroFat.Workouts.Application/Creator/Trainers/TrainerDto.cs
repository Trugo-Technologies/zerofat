using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Application.Creator.Trainers;

public class TrainerSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }
    public string? ProfileMediaUrl { get; set; }
    public List<string>? SpecialisesIn { get; set; }
    public TrainerType Type { get; set; }
    public bool IsActive { get; set; }
}

public class TrainerRawDto : TrainerSimplifyDto
{
}

public class TrainerAuditableDto : TrainerRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class TrainerDto : TrainerAuditableDto
{

}

public class TrainerDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }
    public List<string>? SpecialisesIn { get; set; }
    public TrainerType Type { get; set; }

    public string? BriefAr { get; set; }
    public string? BriefEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? ProfileMediaUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? PinterestUrl { get; set; }
    public string? YoutubeUrl { get; set; }
    public string? WebsiteUrl { get; set; }
}

