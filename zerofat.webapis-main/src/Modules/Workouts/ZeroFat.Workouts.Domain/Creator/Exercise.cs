using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Domain.Creator;

public class Exercise : ActivationEntity, IAggregateRoot, IImageEntity
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public DefaultIdType? TrainerId { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }

    public string? GetOriginalImageUrl() => AvatarImageUrl;
    public string? GetThumbnailUrl() => AvatarImageThumbnailUrl;
    public string? GetOptimizedUrl() => AvatarImageOptimzeUrl;

    public void SetThumbnailUrl(string url) => AvatarImageThumbnailUrl = url;
    public void SetOptimizedUrl(string url) => AvatarImageOptimzeUrl = url;

    public string? MediaUrl { get; set; }
    public string? GifUrl { get; set; }
    public string? InstructionsAr { get; set; }
    public string? InstructionsEn { get; set; }
    public ExerciseType? Type { get; set; }
    public virtual ICollection<WorkoutExercise> Workouts { get; set; }
    public virtual ICollection<ExerciseBodyPart> ExerciseBodyParts { get; set; }
    public virtual Trainer? Trainer { get; set; }
    public Exercise()
    {
        Workouts = [];
        ExerciseBodyParts = [];
    }

}
