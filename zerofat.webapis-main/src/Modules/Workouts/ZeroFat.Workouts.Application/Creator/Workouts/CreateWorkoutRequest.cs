using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Domain;

namespace ZeroFat.GymUp.Application.Creator.Workouts;

public class CreateWorkoutRequest : ICommand<Result<DefaultIdType>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? AvatarImage { get; set; }
    public IFormFile? ProfileMedia { get; set; }
    public string? OverviewAr { get; set; }
    public string? OverviewEn { get; set; }
    public int DurationInMins { get; set; }
    public Level? Level { get; set; }
    public WorkoutFormat? Format { get; set; }
    public GymEnvironment? Environment { get; set; }
    public DefaultIdType? TrainerId { get; set; }
    public DefaultIdType WorkoutTypeId { get; set; }
    public bool? IsActive { get; set; }
    public int CaloriesBurned { get; set; }

    public List<string> SetNamesEn { get; set; } = [];
    public List<string> SetNamesAr { get; set; } = [];
    public List<DefaultIdType>? BodyPartsIds { get; set; }
    public List<DefaultIdType>? EquipmentIds { get; set; }
    public List<ExerciseSetRequest> ExerciseSets { get; set; } = [];

}

public class ExerciseSetRequest
{
    public int SetIndex { get; set; }
    public DefaultIdType ExerciseId { get; set; }
    public int? Reps { get; set; }
    public int? Sets { get; set; }
    public int? DurationInSec { get; set; }
    public int? Weight { get; set; }
}


public class CreateWorkoutRequestValidator : CustomValidator<CreateWorkoutRequest>
{
    public CreateWorkoutRequestValidator(
        IReadRepository<Workout> workoutRepo,
        IReadRepository<Trainer> trainerRepo,
        IReadRepository<BodyPart> bodyPartRepo,
        IReadRepository<Equipment> equipmentRepo,
        IReadRepository<WorkoutType> typeRepo,
        IReadRepository<Exercise> exerciseRepo,
        IStringLocalizer<CreateWorkoutRequestValidator> localizer)
    {
        #region Name Validation

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(localizer["Arabic name is required."])
            .MustAsync(async (req, name, ct) =>
                !await workoutRepo.AnyAsync(new ExpressionSpecification<Workout>(
                    x => x.NameAr == name && x.TrainerId == req.TrainerId), ct))
            .WithMessage(localizer["Arabic name already exists."]);

        RuleFor(u => u.NameEn)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(localizer["English name is required."])
            .MustAsync(async (req, name, ct) =>
                !await workoutRepo.AnyAsync(new ExpressionSpecification<Workout>(
                    x => x.NameEn == name && x.TrainerId == req.TrainerId), ct))
            .WithMessage(localizer["English name already exists."]);

        #endregion

        #region Foreign Keys Validation

        RuleFor(u => u.TrainerId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(localizer["Trainer is required."])
            .MustAsync(async (id, ct) =>
                await trainerRepo.AnyAsync(new ExpressionSpecification<Trainer>(x => x.Id == id), ct))
            .WithMessage(localizer["Trainer not found."]);

        RuleFor(u => u.WorkoutTypeId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(localizer["Workout type is required."])
            .MustAsync(async (id, ct) =>
                await typeRepo.AnyAsync(new ExpressionSpecification<WorkoutType>(x => x.Id == id), ct))
            .WithMessage(localizer["Workout type not found."]);

        #endregion

        #region Collections Validation

        When(x => x.BodyPartsIds != null, () =>
        {
            RuleForEach(u => u.BodyPartsIds)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["Body part is required."])
                .MustAsync(async (id, ct) =>
                    await bodyPartRepo.AnyAsync(new ExpressionSpecification<BodyPart>(x => x.Id == id), ct))
                .WithMessage(localizer["Body part not found."]);
        });

        When(x => x.EquipmentIds != null, () =>
        {
            RuleForEach(u => u.EquipmentIds)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["Equipment is required."])
                .MustAsync(async (id, ct) =>
                    await equipmentRepo.AnyAsync(new ExpressionSpecification<Equipment>(x => x.Id == id), ct))
                .WithMessage(localizer["Equipment not found."]);
        });

        #endregion

        #region Basic Properties Validation
        RuleFor(x => x.DurationInMins)
            .NotEmpty().WithMessage(localizer["Duration is required."])
            .GreaterThan(0).WithMessage(localizer["Duration must be greater than 0."]);

        RuleFor(x => x.AvatarImage)
            .NotEmpty().WithMessage(localizer["Avatar image is required."]);

        RuleFor(x => x.ProfileMedia)
            .NotEmpty().WithMessage(localizer["Profile media is required."])
            .When(x => x.Format == WorkoutFormat.FollowAlong);

        RuleFor(x => x.Level)
            .NotEmpty().WithMessage(localizer["Workout level is required."]);

        RuleFor(x => x.Format)
            .NotEmpty().WithMessage(localizer["Workout format is required."]);
        #endregion

        #region Exercise Sets Validation
        RuleForEach(x => x.ExerciseSets)
            .CustomAsync(async (item, context, cancellationToken) =>
            {
                // Check ExerciseId not empty
                if (item.ExerciseId == Guid.Empty)
                {
                    context.AddFailure(nameof(item.ExerciseId), localizer["Exercise Id is required."]);
                    return;
                }

                // Check Exercise exists
                var exercise = await exerciseRepo.FirstOrDefaultAsync(new ExpressionSpecification<Exercise>(x => x.Id == item.ExerciseId), cancellationToken);

                if (exercise == null)
                {
                    context.AddFailure(nameof(item.ExerciseId), localizer["Invalid Exercise ID."]);
                    return;
                }

                switch (exercise.Type)
                {
                    case ExerciseType.Duration:
                        if (item.DurationInSec.GetValueOrDefault() <= 0)
                        {
                            context.AddFailure(localizer["Duration must be greater than 0."]);
                        }
                        break;

                    case ExerciseType.Reps:
                        if (item.Reps.GetValueOrDefault() <= 0)
                        {
                            context.AddFailure(localizer["Reps must be greater than 0."]);
                        }
                        break;

                    case ExerciseType.WeightAndReps:
                        if (item.Weight.GetValueOrDefault() <= 0 || item.Reps.GetValueOrDefault() <= 0)
                        {
                            context.AddFailure(localizer["Weight and reps must be greater than 0."]);
                        }
                        break;

                    default:
                        break;
                }
            });
        #endregion

    }
}

public class CreateWorkoutRequestHandler(IRepositoryWithEvents<Workout> repository, IFileStorageManager uploadFile) : IRequestHandler<CreateWorkoutRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Workout> _repository = repository;
    private readonly IFileStorageManager _uploadFile = uploadFile;


    public async Task<Result<DefaultIdType>> Handle(CreateWorkoutRequest request, CancellationToken cancellationToken)
    {
        var workout = new Workout
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            DurationInMins = request.DurationInMins,
            Format = request.Format!.Value,
            TrainerId = request.TrainerId,
            Level = request.Level!.Value,
            OverviewAr = request.OverviewAr,
            OverviewEn = request.OverviewEn,
            WorkoutTypeId = request.WorkoutTypeId,

            ProfileMediaUrl = await _uploadFile.UploadAsync<Workout>(request.ProfileMedia, FileType.Other, ModuleConstant.ModuleName, cancellationToken),
            IsActive = request.IsActive ?? false,
            SetNamesAr = request.SetNamesAr,
            SetNamesEn = request.SetNamesEn,
            CaloriesBurned = request.CaloriesBurned,
        };

        if (request.AvatarImage != null)
        {
            workout.AvatarImageUrl = await _uploadFile.UploadAsync<Workout>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            workout.AvatarImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Workout>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            workout.AvatarImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Workout>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        if (request.BodyPartsIds?.Count > 0)
        {
            workout.WorkoutBodyParts = request.BodyPartsIds.ConvertAll(id => new WorkoutBodyPart
            {
                BodyPartId = id
            });
        }

        if (request.EquipmentIds?.Count > 0)
        {
            workout.WorkoutEquipments = request.EquipmentIds.ConvertAll(id => new WorkoutEquipment
            {
                EquipmentId = id
            });
        }

        if (request.ExerciseSets?.Count > 0)
        {
            int index = 1;

            foreach (var exercise in request.ExerciseSets.OrderBy(x => x.SetIndex))
            {
                workout.WorkoutExercises.Add(new WorkoutExercise
                {
                    ExerciseId = exercise.ExerciseId,
                    SetIndex = exercise.SetIndex,
                    Index = index,
                    DurationInSec = exercise.DurationInSec,
                    Weight = exercise.Weight,
                    Sets = exercise.Sets,
                    Reps = exercise.Reps,
                    SetNameAr = request.SetNamesAr[exercise.SetIndex],
                    SetNameEn = request.SetNamesEn[exercise.SetIndex]
                });

                index++;
            }
        }

        await _repository.AddAsync(workout, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(workout.Id);
    }

}
