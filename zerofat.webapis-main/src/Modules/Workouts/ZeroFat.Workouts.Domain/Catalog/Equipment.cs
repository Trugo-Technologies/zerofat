using ZeroFat.GymUp.Domain.Creator;

namespace ZeroFat.GymUp.Domain.Catalog;


public class Equipment : ActivationEntity, IAggregateRoot, IImageEntity
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public string? ImageOptimzeUrl { get; set; }
    public DefaultIdType CategoryId { get; set; }
    public virtual EquipmentCategory Category { get; set; } = default!;
    public virtual ICollection<WorkoutEquipment> Workouts { get; set; }

    public string? GetOriginalImageUrl() => ImageUrl;
    public string? GetThumbnailUrl() => ImageThumbnailUrl;
    public string? GetOptimizedUrl() => ImageOptimzeUrl;

    public void SetThumbnailUrl(string url) => ImageThumbnailUrl = url;
    public void SetOptimizedUrl(string url) => ImageOptimzeUrl = url;

    public Equipment()
    {
        Workouts = new HashSet<WorkoutEquipment>();
    }
}
