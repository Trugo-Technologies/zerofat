using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Application.Catalog.BodyParts;
using ZeroFat.GymUp.Application.Catalog.EquipmentCategories;
using ZeroFat.GymUp.Application.Catalog.Equipments;
using ZeroFat.GymUp.Application.Catalog.WorkoutTypes;
using ZeroFat.GymUp.Application.Creator.ClientWorkouts;
using ZeroFat.GymUp.Application.Creator.Trainers;
using ZeroFat.GymUp.Application.Creator.WorkoutExercises;

namespace ZeroFat.GymUp.Application.Creator.Workouts;

public class WorkoutSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }
    public string? ProfileMediaUrl { get; set; }
    public int DurationInMins { get; set; }
    public Level Level { get; set; }
    public bool IsActive { get; set; }
    public int CaloriesBurned { get; set; }


}

public class WorkoutRawDto : WorkoutSimplifyDto
{

    public WorkoutFormat Format { get; set; }

    public DefaultIdType? TrainerId { get; set; }
    public DefaultIdType WorkoutTypeId { get; set; }


}

public class WorkoutAuditableDto : WorkoutRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class WorkoutDto : WorkoutAuditableDto
{
    public TrainerSimplifyDto? Trainer { get; set; }
    public WorkoutTypeSimplifyDto? WorkoutType { get; set; }
    public List<BodyPartSimplifyDto>? BodyParts { get; set; }
    public List<EquipmentSimplifyDto>? Equipments { get; set; }
    public ClientWorkoutSimplifyDto? ClientWorkout { get; set; }
    public List<string>? SetNamesEn { get; set; }
    public List<string>? SetNamesAr { get; set; }
}

public class WorkoutDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }
    public string? ProfileMediaUrl { get; set; }
    public int DurationInMins { get; set; }
    public Level Level { get; set; }
    public int CaloriesBurned { get; set; }

    public WorkoutFormat Format { get; set; }
    public GymEnvironment? Environment { get; set; }

    public DefaultIdType? TrainerId { get; set; }
    public DefaultIdType WorkoutTypeId { get; set; }

    public string? OverviewAr { get; set; }
    public string? OverviewEn { get; set; }


    public List<string>? SetNamesEn { get; set; }
    public List<string>? SetNamesAr { get; set; }

    public TrainerSimplifyDto? Trainer { get; set; }
    public ClientWorkoutSimplifyDto? ClientWorkout { get; set; }
    public List<BodyPartSimplifyDto>? BodyParts { get; set; }
    public List<EquipmentSimplifyDto>? Equipments { get; set; }
    public WorkoutTypeSimplifyDto? WorkoutType { get; set; }
    public List<DefaultIdType>? BodyPartIds { get; set; }
    public List<DefaultIdType>? EquipmentIds { get; set; }
}

public class WorkoutMobileDetailsDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? AvatarImageThumbnailUrl { get; set; }
    public string? AvatarImageOptimzeUrl { get; set; }
    public string? ProfileMediaUrl { get; set; }
    public int DurationInMins { get; set; }
    public int CaloriesBurned { get; set; }
    public Level Level { get; set; }
    public WorkoutFormat Format { get; set; }
    public GymEnvironment? Environment { get; set; }

    public string? OverviewAr { get; set; }
    public string? OverviewEn { get; set; }

    public ClientWorkoutSimplifyDto? ClientWorkout { get; set; }
    public List<EquipmentSimplifyDto>? Equipments { get; set; }
    public List<WorkoutExerciseSimplifyDto>? WorkoutExercises { get; set; }
}

