using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Domain;

namespace ZeroFat.GymUp.Application.Creator.Exercises;
public class CreateExerciseRequest : ICommand<Result<DefaultIdType>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? AvatarImage { get; set; }
    public IFormFile? Media { get; set; }
    public IFormFile? Gif { get; set; }
    public string? InstructionsAr { get; set; }
    public string? InstructionsEn { get; set; }
    public DefaultIdType? TrainerId { get; set; }
    public ExerciseType? Type { get; set; }

    public List<DefaultIdType>? BodyPartsIds { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateExerciseRequestValidator : CustomValidator<CreateExerciseRequest>
{
    public CreateExerciseRequestValidator(IReadRepository<Exercise> repository, IReadRepository<Trainer> trainerRepo, IReadRepository<BodyPart> bodyPartRepo, IStringLocalizer<CreateExerciseRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();


        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty();

        RuleFor(u => u.Type)
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

        RuleFor(x => x.Gif)
            .NotEmpty();

        RuleFor(x => x.AvatarImage)
            .NotEmpty();

    }
}


public class CreateExerciseRequestHandler(IRepositoryWithEvents<Exercise> repository, IFileStorageManager uploadFile) : IRequestHandler<CreateExerciseRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Exercise> _repository = repository;
    private readonly IFileStorageManager _uploadFile = uploadFile;


    public async Task<Result<DefaultIdType>> Handle(CreateExerciseRequest request, CancellationToken cancellationToken)
    {
        var exercise = new Exercise
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            AvatarImageUrl = await _uploadFile.UploadAsync<Exercise>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
            MediaUrl = await _uploadFile.UploadAsync<Exercise>(request.Media, FileType.Other, ModuleConstant.ModuleName, cancellationToken),
            GifUrl = await _uploadFile.UploadAsync<Exercise>(request.Gif, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
            InstructionsEn = request.InstructionsEn,
            InstructionsAr = request.InstructionsAr,
            TrainerId = request.TrainerId,
            Type = request.Type,
            IsActive = request.IsActive ?? false
        };

        if (request.AvatarImage != null)
        {
            exercise.AvatarImageUrl = await _uploadFile.UploadAsync<Exercise>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            exercise.AvatarImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Exercise>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            exercise.AvatarImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Exercise>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        if (request.BodyPartsIds != null)
        {
            foreach (var id in request.BodyPartsIds)
                exercise.ExerciseBodyParts.Add(new ExerciseBodyPart
                {
                    BodyPartId = id
                });
        }

        await _repository.AddAsync(exercise, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(exercise.Id);
    }

}
