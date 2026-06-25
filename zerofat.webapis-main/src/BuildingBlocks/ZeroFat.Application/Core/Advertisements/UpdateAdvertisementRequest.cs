using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Core.Advertisements;
public class UpdateAdvertisementRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public string? TitleEn { get; set; }
    public string? TitleAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? WelcomeMsgAr { get; set; }
    public string? WelcomeMsgEn { get; set; }
    public string? ImageUrl { get; set; }
    public int Index { get; set; }
    public IFormFile? Image { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateAdvertisementRequestValidator : CustomValidator<UpdateAdvertisementRequest>
{
    public UpdateAdvertisementRequestValidator(IReadRepository<Advertisement> repository, IStringLocalizer<UpdateAdvertisementRequestValidator> localaizer)
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

        RuleFor(u => u.Index)
           .Cascade(CascadeMode.Stop)
           .NotEmpty();
    }
}

public class UpdateAdvertisementRequestHandler(IRepository<Advertisement> repository, IStringLocalizer<UpdateAdvertisementRequestHandler> localizer, IFileStorageManager uploadFile) : IRequestHandler<UpdateAdvertisementRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Advertisement> _repository = repository;
    private readonly IFileStorageManager _uploadFile = uploadFile;
    private readonly IStringLocalizer<UpdateAdvertisementRequestHandler> _localizer = localizer;


    public async Task<Result<DefaultIdType>> Handle(UpdateAdvertisementRequest request, CancellationToken cancellationToken)
    {
        var adv = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = adv ?? throw new NotFoundException(_localizer["Advertisement not found"]);

        if (request.Index != adv.Index)
        {
            var advs = await _repository.ListAsync(new ExpressionSpecification<Advertisement>(x => x.Index >= request.Index && x.Id != request.Id), cancellationToken);
            foreach (var item in advs)
                item.Index++;
        }

        adv.TitleEn = request.TitleEn;
        adv.TitleAr = request.TitleAr;
        adv.WelcomeMsgEn = request.WelcomeMsgEn;
        adv.WelcomeMsgAr = request.WelcomeMsgAr;
        adv.DescriptionAr = request.DescriptionAr;
        adv.DescriptionEn = request.DescriptionEn;
        adv.IsActive = request.IsActive;
        adv.Index = request.Index;

        if (request.Image != null)
        {
            adv.ImageUrl = await _uploadFile.UploadAsync<Advertisement>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            adv.ImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Advertisement>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            adv.ImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Advertisement>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        await _repository.SaveChangesAsync(cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(adv.Id);
    }
}

