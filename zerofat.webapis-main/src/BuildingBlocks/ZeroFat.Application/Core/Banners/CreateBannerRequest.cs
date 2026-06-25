using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Core.Banners;
public class CreateBannerRequest : ICommand<Result<DefaultIdType>>
{
    public string? TitleEn { get; set; }
    public string? TitleAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public IFormFile? Image { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateBannerRequestValidator : CustomValidator<CreateBannerRequest>
{
    public CreateBannerRequestValidator(
        IReadRepository<Banner> repository, 
        IStringLocalizer<CreateBannerRequestValidator> localaizer)
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


public class CreateBannerRequestHandler(IRepository<Banner> repo, IFileStorageManager uploadFile) : IRequestHandler<CreateBannerRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Banner> _repo = repo;
    private readonly IFileStorageManager _uploadFile = uploadFile;

    public async Task<Result<DefaultIdType>> Handle(CreateBannerRequest request, CancellationToken cancellationToken)
    {
        var lastBanner = await _repo.FirstOrDefaultAsync(new LastBannerByIndexSpec(),cancellationToken);
        var adv = new Banner
        {
            TitleEn = request.TitleEn,
            TitleAr = request.TitleAr,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            ImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Banner>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
            IsActive = request.IsActive ?? false,
            Index = (lastBanner?.Index ?? 0) + 1,
        };

        if (request.Image != null)
        {
            adv.ImageUrl = await _uploadFile.UploadAsync<Banner>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            adv.ImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Banner>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            adv.ImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Banner>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        await _repo.AddAsync(adv, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(adv.Id);
    }

}
