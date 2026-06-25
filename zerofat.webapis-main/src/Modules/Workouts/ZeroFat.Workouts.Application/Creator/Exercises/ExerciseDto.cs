using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Application.Catalog.BodyParts;
using ZeroFat.GymUp.Application.Creator.Trainers;

namespace ZeroFat.GymUp.Application.Creator.Exercises;

public class ExerciseSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? InstructionsAr { get; set; }
    public string? InstructionsEn { get; set; }

    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }

    public string? MediaUrl { get; set; }
    public string? GifUrl { get; set; }
    public ExerciseType? Type { get; set; }
    public bool IsActive { get; set; }
}

public class ExerciseRawDto : ExerciseSimplifyDto
{
    public Guid? TrainerId { get; set; }

}

public class ExerciseAuditableDto : ExerciseRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class ExerciseDto : ExerciseAuditableDto
{
    public TrainerSimplifyDto? Trainer { get; set; }
    public List<BodyPartSimplifyDto>? BodyParts { get; set; }
}

public class ExerciseDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }
    public ExerciseType? Type { get; set; }

    public Guid? TrainerId { get; set; }
    public string? MediaUrl { get; set; }
    public string? GifUrl { get; set; }
    public string? InstructionsAr { get; set; }
    public string? InstructionsEn { get; set; }
    public TrainerSimplifyDto? Trainer { get; set; }
    public List<BodyPartSimplifyDto>? BodyParts { get; set; }
    public List<Guid>? BodyPartIds { get; set; }
}

