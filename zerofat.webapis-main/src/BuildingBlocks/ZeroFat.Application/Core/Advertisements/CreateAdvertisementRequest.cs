using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Core.Advertisements;
public class CreateAdvertisementRequest : ICommand<Result<DefaultIdType>>
{
    public string? TitleEn { get; set; }
    public string? TitleAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? WelcomeMsgAr { get; set; }
    public string? WelcomeMsgEn { get; set; }
    public IFormFile? Image { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateAdvertisementRequestValidator : CustomValidator<CreateAdvertisementRequest>
{
    public CreateAdvertisementRequestValidator(IReadRepository<Advertisement> repository, IStringLocalizer<CreateAdvertisementRequestValidator> localaizer)
    {

        RuleFor(u => u.TitleAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();

        RuleFor(u => u.TitleEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty();

        RuleFor(u => u.DescriptionAr)
           .Cascade(CascadeMode.Stop)
           .NotEmpty();

        RuleFor(u => u.DescriptionEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty();

    }
}


public class CreateAdvertisementRequestHandler(IRepository<Advertisement> repo, IFileStorageManager uploadFile) : IRequestHandler<CreateAdvertisementRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Advertisement> _repo = repo;
    private readonly IFileStorageManager _uploadFile = uploadFile;

    public async Task<Result<DefaultIdType>> Handle(CreateAdvertisementRequest request, CancellationToken cancellationToken)
    {
        var lastAdvertisement = await _repo.FirstOrDefaultAsync(new LastAdvertisementByIndexSpec(),cancellationToken);
        var adv = new Advertisement
        {
            TitleEn = request.TitleEn,
            TitleAr = request.TitleAr,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            WelcomeMsgAr = request.WelcomeMsgAr,
            WelcomeMsgEn = request.WelcomeMsgEn,
            IsActive = request.IsActive ?? false,
            Index = (lastAdvertisement?.Index ?? 0) + 1,
        };

        if (request.Image != null)
        {
            adv.ImageUrl = await _uploadFile.UploadAsync<Advertisement>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            adv.ImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Advertisement>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            adv.ImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Advertisement>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        await _repo.AddAsync(adv, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(adv.Id);
    }

}
