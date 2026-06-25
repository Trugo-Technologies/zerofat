using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Domain.Creator;

public class Trainer : ActivationEntity, IAggregateRoot, IImageEntity
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;

    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }

    public string? GetOriginalImageUrl() => AvatarImageUrl;
    public string? GetThumbnailUrl() => AvatarImageThumbnailUrl;
    public string? GetOptimizedUrl() => AvatarImageOptimzeUrl;

    public void SetThumbnailUrl(string url) => AvatarImageThumbnailUrl = url;
    public void SetOptimizedUrl(string url) => AvatarImageOptimzeUrl = url;


    public string? ProfileMediaUrl { get; set; }
    public string? BriefAr { get; set; }
    public string? BriefEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public TrainerType Type { get; set; }

    public List<string> SpecialisesIn { get; set; }

    public string? InstagramUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? PinterestUrl { get; set; }
    public string? YoutubeUrl { get; set; }
    public string? WebsiteUrl { get; set; }


    public virtual ICollection<Plan>? Plans { get; set; }

    public Trainer()
    {
        SpecialisesIn = [];
        Plans = [];
    }
}
