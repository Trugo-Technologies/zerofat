using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Domain;
using ZeroFat.GymUp.Domain.Creator;

namespace ZeroFat.GymUp.Application.Creator.Trainers;
public class CreateTrainerRequest : ICommand<Result<DefaultIdType>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? AvatarImage { get; set; }
    public IFormFile? ProfileMedia { get; set; }
    public string? BriefAr { get; set; }
    public string? BriefEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public TrainerType? Type { get; set; }
    public List<string> SpecialisesIn { get; set; }
    public string? InstagramUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? PinterestUrl { get; set; }
    public string? YoutubeUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateTrainerRequestValidator : CustomValidator<CreateTrainerRequest>
{
    public CreateTrainerRequestValidator(IReadRepository<Trainer> repository, IStringLocalizer<CreateTrainerRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<Trainer>(x => x.NameAr == name), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<Trainer>(x => x.NameEn == name), _))
                .WithMessage(localaizer["English name already exists"]);

        RuleFor(x => x.Type)
            .NotEmpty();

        RuleFor(x => x.AvatarImage)
            .NotEmpty();

        RuleFor(x => x.ProfileMedia)
            .NotEmpty();

        RuleFor(x => x.BriefAr)
            .NotEmpty();

        RuleFor(x => x.BriefEn)
            .NotEmpty();

        RuleFor(x => x.SpecialisesIn)
            .NotEmpty();

    }
}


public class CreateTrainerRequestHandler(IRepositoryWithEvents<Trainer> repository, IFileStorageManager uploadFile) : IRequestHandler<CreateTrainerRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Trainer> _repository = repository;
    private readonly IFileStorageManager _uploadFile = uploadFile;


    public async Task<Result<DefaultIdType>> Handle(CreateTrainerRequest request, CancellationToken cancellationToken)
    {
        var trainer = new Trainer
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            BriefAr = request.BriefAr,
            BriefEn = request.BriefEn,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            Type = request.Type!.Value,
            SpecialisesIn = request.SpecialisesIn,
            InstagramUrl = request.InstagramUrl,
            FacebookUrl = request.FacebookUrl,
            PinterestUrl = request.PinterestUrl,
            YoutubeUrl = request.YoutubeUrl,
            WebsiteUrl = request.WebsiteUrl,
            ProfileMediaUrl = await _uploadFile.UploadAsync<Trainer>(request.ProfileMedia, FileType.Other, ModuleConstant.ModuleName, cancellationToken),
            IsActive = request.IsActive ?? false
        };

        if (request.AvatarImage != null)
        {
            trainer.AvatarImageUrl = await _uploadFile.UploadAsync<Trainer>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            trainer.AvatarImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Trainer>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            trainer.AvatarImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Trainer>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        if (request.AvatarImage != null)
        {
            trainer.AvatarImageUrl = await _uploadFile.UploadAsync<Trainer>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            trainer.AvatarImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Trainer>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            trainer.AvatarImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Trainer>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        await _repository.AddAsync(trainer, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(trainer.Id);
    }

}
