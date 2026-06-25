using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.GymUp.Domain;

namespace ZeroFat.GymUp.Application.Creator.Exercises;
public class UpdateExerciseRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? AvatarImageUrl { get; set; }
    public string? MediaUrl { get; set; }
    public string? GifUrl { get; set; }
    public IFormFile? AvatarImage { get; set; }
    public IFormFile? Media { get; set; }
    public IFormFile? Gif { get; set; }
    public string? InstructionsAr { get; set; }
    public string? InstructionsEn { get; set; }
    public DefaultIdType? TrainerId { get; set; }
    public List<DefaultIdType>? BodyPartsIds { get; set; }

    public bool IsActive { get; set; }
}

public class UpdateExerciseRequestValidator : CustomValidator<UpdateExerciseRequest>
{
    public UpdateExerciseRequestValidator(IReadRepository<Exercise> repository, IReadRepository<Trainer> trainerRepo, IReadRepository<BodyPart> bodyPartRepo, IStringLocalizer<UpdateExerciseRequestValidator> localaizer)
    {
        RuleFor(u => u.NameAr)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty();

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty();

        When(x => x.TrainerId.HasValue, () =>
        {
            RuleFor(u => u.TrainerId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await trainerRepo.AnyAsync(new ExpressionSpecification<Trainer>(x => x.Id == id), _))
                .WithMessage(localaizer["Trainer not found"]);
        });

        When(x => x.BodyPartsIds != null, () =>
        {
            RuleForEach(u => u.BodyPartsIds)
              .Cascade(CascadeMode.Stop)
              .NotEmpty()
              .MustAsync(async (id, _) => await bodyPartRepo.AnyAsync(new ExpressionSpecification<BodyPart>(x => x.Id == id), _))
                   .WithMessage(localaizer["Body part not found"]);
        });
    }
}

public class UpdateExerciseRequestHandler(IRepositoryWithEvents<Exercise> repository, IStringLocalizer<UpdateExerciseRequestHandler> localizer, IFileStorageManager uploadFile, IRepositoryWithEvents<ExerciseBodyPart> exerciseBodyRepo) : IRequestHandler<UpdateExerciseRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Exercise> _repository = repository;
    private readonly IRepositoryWithEvents<ExerciseBodyPart> _exerciseBodyRepo = exerciseBodyRepo;
    private readonly IStringLocalizer<UpdateExerciseRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _uploadFile = uploadFile;


    public async Task<Result<DefaultIdType>> Handle(UpdateExerciseRequest request, CancellationToken cancellationToken)
    {
        var exercise = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = exercise ?? throw new NotFoundException(_localizer["Exercise not found"]);
        exercise.NameAr = request.NameAr;
        exercise.NameEn = request.NameEn;
        exercise.InstructionsEn = request.InstructionsEn;
        exercise.InstructionsAr = request.InstructionsAr;
        exercise.TrainerId = request.TrainerId;

        exercise.MediaUrl = request.Media != null ? await _uploadFile.UploadAsync<Exercise>(request.Media, FileType.Other, ModuleConstant.ModuleName, cancellationToken) : request.MediaUrl == exercise.MediaUrl ? exercise.MediaUrl : null;
        exercise.GifUrl = request.Gif != null ? await _uploadFile.UploadAsync<Exercise>(request.Gif, FileType.Image, ModuleConstant.ModuleName, cancellationToken) : request.GifUrl == exercise.GifUrl ? exercise.GifUrl : null;
        exercise.IsActive = request.IsActive;

        if (request.AvatarImage != null)
        {
            exercise.AvatarImageUrl = await _uploadFile.UploadAsync<Exercise>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            exercise.AvatarImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Exercise>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            exercise.AvatarImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Exercise>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        var bodyParts = await _exerciseBodyRepo.ListAsync(new ExpressionSpecification<ExerciseBodyPart>(x => x.ExerciseId == exercise.Id), cancellationToken);
        if (bodyParts.Count() != 0)
            await _exerciseBodyRepo.DeleteRangeAsync(bodyParts, cancellationToken);

        if (request.BodyPartsIds != null)
        {
            foreach (var id in request.BodyPartsIds)
                await _exerciseBodyRepo.AddAsync(new ExerciseBodyPart
                {
                    BodyPartId = id,
                    ExerciseId = exercise.Id
                });
        }

        await _repository.UpdateAsync(exercise, withSaveChanges: true, cancellationToken: cancellationToken);
        return await Result<DefaultIdType>.SuccessAsync(exercise.Id);
    }
}

