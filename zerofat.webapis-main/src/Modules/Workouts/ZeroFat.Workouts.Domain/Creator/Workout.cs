using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Domain.Catalog;

namespace ZeroFat.GymUp.Domain.Creator;
public class Workout : ActivationEntity, IAggregateRoot, IImageEntity
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
    public string? OverviewAr { get; set; }
    public string? OverviewEn { get; set; }
    public int DurationInMins { get; set; }
    public int CaloriesBurned { get; set; }
    public Level Level { get; set; }
    public WorkoutFormat Format { get; set; }



    public DefaultIdType? TrainerId { get; set; }
    public virtual Trainer? Trainer { get; set; }

    public DefaultIdType WorkoutTypeId { get; set; }
    public virtual WorkoutType WorkoutType { get; set; } = default!;

    //public string? InstagramUrl { get; set; }
    //public string? FacebookUrl { get; set; }
    //public string? PinterestUrl { get; set; }
    //public string? YoutubeUrl { get; set; }
    //public string? WebsiteUrl { get; set; }

    public List<string>? SetNamesEn { get; set; }
    public List<string>? SetNamesAr { get; set; }


    public virtual ICollection<PlanSchedule> PlanSchedules { get; set; }
    public virtual ICollection<WorkoutEquipment> WorkoutEquipments { get; set; }
    public virtual ICollection<WorkoutBodyPart> WorkoutBodyParts { get; set; }
    public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; }
    public Workout()
    {
        PlanSchedules = [];
        WorkoutEquipments = [];
        WorkoutBodyParts = [];
        WorkoutExercises = [];
    }
}
