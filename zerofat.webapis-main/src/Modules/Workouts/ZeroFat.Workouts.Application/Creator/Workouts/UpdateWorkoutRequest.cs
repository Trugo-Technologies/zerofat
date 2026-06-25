using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Domain;
using ZeroFat.GymUp.Domain.Creator;

namespace ZeroFat.GymUp.Application.Creator.Workouts;
public class UpdateWorkoutRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? AvatarImage { get; set; }
    public IFormFile? ProfileMedia { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? ProfileMediaUrl { get; set; }
    public string? OverviewAr { get; set; }
    public string? OverviewEn { get; set; }
    public int DurationInMins { get; set; }
    public int CaloriesBurned { get; set; }
    public Level Level { get; set; }
    public DefaultIdType? TrainerId { get; set; }
    public DefaultIdType WorkoutTypeId { get; set; }
    public List<string>? SetNamesEn { get; set; }
    public List<string>? SetNamesAr { get; set; }
    public bool IsActive { get; set; }
    public List<DefaultIdType>? BodyPartsIds { get; set; }
    public List<DefaultIdType>? EquipmentIds { get; set; }

}

public class UpdateWorkoutRequestValidator : CustomValidator<UpdateWorkoutRequest>
{
    public UpdateWorkoutRequestValidator(
        IReadRepository<Workout> repository,
        IReadRepository<Trainer> trainerRepo,
        IReadRepository<BodyPart> bodyPartRepo,
        IReadRepository<Equipment> equipmentRepo,
        IReadRepository<WorkoutType> typeRepo,
        IStringLocalizer<UpdateWorkoutRequestValidator> localizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Workout>(x => x.NameAr == name && x.TrainerId == req.TrainerId && x.Id != req.Id), _))
                 .WithMessage(localizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Workout>(x => x.NameEn == name && x.TrainerId == req.TrainerId && x.Id != req.Id), _))
                .WithMessage(localizer["English name already exists"]);

        RuleFor(u => u.TrainerId)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .MustAsync(async (id, _) => await trainerRepo.AnyAsync(new ExpressionSpecification<Trainer>(x => x.Id == id), _))
               .WithMessage(localizer["Trainer not found"]);

        RuleFor(u => u.WorkoutTypeId)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .MustAsync(async (id, _) => await typeRepo.AnyAsync(new ExpressionSpecification<WorkoutType>(x => x.Id == id), _))
               .WithMessage(localizer["Type not found"]);

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


        RuleFor(x => x.DurationInMins)
            .NotEmpty()
            .GreaterThan(0)
                .WithMessage(localizer["Duration must be greater than 0"]);

    }
}

public class UpdateWorkoutRequestHandler(
    IRepositoryWithEvents<Workout> repository,
    IRepositoryWithEvents<WorkoutEquipment> workoutEquipmentRepo,
    IStringLocalizer<UpdateWorkoutRequestHandler> localizer,
    IFileStorageManager uploadFile, 
    IRepositoryWithEvents<WorkoutBodyPart> workoutBodyRepo) : IRequestHandler<UpdateWorkoutRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Workout> _repository = repository;
    private readonly IRepositoryWithEvents<WorkoutBodyPart> _workoutBodyRepo = workoutBodyRepo;
    private readonly IRepositoryWithEvents<WorkoutEquipment> _workoutEquipmentRepo = workoutEquipmentRepo;
    private readonly IStringLocalizer<UpdateWorkoutRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _uploadFile = uploadFile;


    public async Task<Result<DefaultIdType>> Handle(UpdateWorkoutRequest request, CancellationToken cancellationToken)
    {
        var workout = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = workout ?? throw new NotFoundException(_localizer["Workout not found"]);

        workout.NameAr = request.NameAr;
        workout.NameEn = request.NameEn;
        workout.DurationInMins = request.DurationInMins;
        workout.TrainerId = request.TrainerId;
        workout.Level = request.Level;
        workout.OverviewAr = request.OverviewAr;
        workout.CaloriesBurned = request.CaloriesBurned;
        workout.SetNamesAr = request.SetNamesAr;
        workout.SetNamesEn = request.SetNamesEn;
        workout.OverviewEn = request.OverviewEn;
        workout.WorkoutTypeId = request.WorkoutTypeId;
        workout.ProfileMediaUrl = request.ProfileMedia != null ? await _uploadFile.UploadAsync<Workout>(request.ProfileMedia, FileType.Other, ModuleConstant.ModuleName, cancellationToken) : request.ProfileMediaUrl == workout.ProfileMediaUrl ? workout.ProfileMediaUrl : null;
        workout.IsActive = request.IsActive;

        if (request.AvatarImage != null)
        {
            workout.AvatarImageUrl = await _uploadFile.UploadAsync<Workout>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            workout.AvatarImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Workout>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            workout.AvatarImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Workout>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        await _workoutBodyRepo.SyncRelationAsync(
                new ExpressionSpecification<WorkoutBodyPart>(x => x.WorkoutId == workout.Id),
                request.BodyPartsIds,
                x => x.BodyPartId,
                id => new WorkoutBodyPart { WorkoutId = workout.Id, BodyPartId = id },
                cancellationToken);

        await _workoutEquipmentRepo.SyncRelationAsync(
               new ExpressionSpecification<WorkoutEquipment>(x => x.WorkoutId == workout.Id),
               request.EquipmentIds,
               x => x.EquipmentId,
               id => new WorkoutEquipment { WorkoutId = workout.Id, EquipmentId = id },
               cancellationToken);

        await _repository.UpdateAsync(workout, withSaveChanges: true, cancellationToken: cancellationToken);
        return await Result<DefaultIdType>.SuccessAsync(workout.Id);
    }
}

