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

namespace ZeroFat.GymUp.Application.Creator.Trainers;
public class UpdateTrainerRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? AvatarImage { get; set; }
    public IFormFile? ProfileMedia { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? ProfileMediaUrl { get; set; }
    public string? BriefAr { get; set; }
    public string? BriefEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public TrainerType? Type { get; set; }
    public List<string> SpecialisesIn { get; set; } = new();
    public string? InstagramUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? PinterestUrl { get; set; }
    public string? YoutubeUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateTrainerRequestValidator : CustomValidator<UpdateTrainerRequest>
{
    public UpdateTrainerRequestValidator(IReadRepository<Trainer> repository, IStringLocalizer<UpdateTrainerRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Trainer>(x => x.NameAr == name && req.Id != x.Id), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Trainer>(x => x.NameEn == name && req.Id != x.Id), _))
                .WithMessage(localaizer["English name already exists"]);

        RuleFor(x => x.Type)
             .NotEmpty();

        RuleFor(x => x.BriefAr)
            .NotEmpty();

        RuleFor(x => x.BriefEn)
            .NotEmpty();

        RuleFor(x => x.SpecialisesIn)
            .NotEmpty();

    }
}

public class UpdateTrainerRequestHandler(IRepositoryWithEvents<Trainer> repository, IStringLocalizer<UpdateTrainerRequestHandler> localizer, IFileStorageManager uploadFile) : IRequestHandler<UpdateTrainerRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Trainer> _repository = repository;
    private readonly IStringLocalizer<UpdateTrainerRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _uploadFile = uploadFile;


    public async Task<Result<DefaultIdType>> Handle(UpdateTrainerRequest request, CancellationToken cancellationToken)
    {
        var trainer = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = trainer ?? throw new NotFoundException(_localizer["Trainer not found"]);
        trainer.NameAr = request.NameAr;
        trainer.NameEn = request.NameEn;
        trainer.BriefAr = request.BriefAr;
        trainer.BriefEn = request.BriefEn;
        trainer.DescriptionAr = request.DescriptionAr;
        trainer.DescriptionEn = request.DescriptionEn;
        trainer.Type = request.Type!.Value;
        trainer.SpecialisesIn = request.SpecialisesIn;
        trainer.InstagramUrl = request.InstagramUrl;
        trainer.FacebookUrl = request.FacebookUrl;
        trainer.PinterestUrl = request.PinterestUrl;
        trainer.YoutubeUrl = request.YoutubeUrl;
        trainer.WebsiteUrl = request.WebsiteUrl;
        trainer.ProfileMediaUrl = request.ProfileMedia != null ? await _uploadFile.UploadAsync<Trainer>(request.ProfileMedia, FileType.Other, ModuleConstant.ModuleName, cancellationToken) : request.ProfileMediaUrl == trainer.ProfileMediaUrl ? trainer.ProfileMediaUrl : null;
        trainer.IsActive = request.IsActive;

        if (request.AvatarImage != null)
        {
            trainer.AvatarImageUrl = await _uploadFile.UploadAsync<Trainer>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            trainer.AvatarImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Trainer>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            trainer.AvatarImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Trainer>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        await _repository.UpdateAsync(trainer, withSaveChanges: true, cancellationToken: cancellationToken);
        return await Result<DefaultIdType>.SuccessAsync(trainer.Id);
    }
}

