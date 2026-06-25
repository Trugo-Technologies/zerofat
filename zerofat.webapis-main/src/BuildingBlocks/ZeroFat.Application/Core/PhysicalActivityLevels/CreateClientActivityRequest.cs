using MediatR;
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

namespace ZeroFat.Application.Core.PhysicalActivityLevels;
public class CreatePhysicalActivityLevelRequest : ICommand<Result<Guid>>
{
    public IFormFile? Image { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? ExampleEn { get; set; }
    public string? ExampleAr { get; set; }
    public double ActivityValue { get; set; }
    public bool? IsActive { get; set; }
}

public class CreatePhysicalActivityLevelRequestValidator : CustomValidator<CreatePhysicalActivityLevelRequest>
{
    public CreatePhysicalActivityLevelRequestValidator(IReadRepository<PhysicalActivityLevel> repository, IStringLocalizer<CreatePhysicalActivityLevelRequestValidator> localaizer)
    {

        RuleFor(u => u.Image)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(u => u.ActivityValue)
           .Cascade(CascadeMode.Stop)
           .GreaterThan(1)
           .WithMessage(localaizer["ActivityValue must be more than 1"]);
    }
}


public class CreatePhysicalActivityLevelRequestHandler(IRepository<PhysicalActivityLevel> repo, IFileStorageManager uploadFile) : IRequestHandler<CreatePhysicalActivityLevelRequest, Result<Guid>>
{
    private readonly IRepository<PhysicalActivityLevel> _repo = repo;
    private readonly IFileStorageManager _uploadFile = uploadFile;


    public async Task<Result<Guid>> Handle(CreatePhysicalActivityLevelRequest request, CancellationToken cancellationToken)
    {
        var activity = new PhysicalActivityLevel
        {
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            ExampleEn = request.ExampleEn,
            ExampleAr = request.ExampleAr,
            ActivityValue = request.ActivityValue,
            ImageUrl = await _uploadFile.UploadAsync<PhysicalActivityLevel>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
            IsActive = request.IsActive ?? false
        };

        if (request.Image != null)
        {
            activity.ImageUrl = await _uploadFile.UploadAsync<PhysicalActivityLevel>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            activity.ImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<PhysicalActivityLevel>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            activity.ImageOptimzeUrl = await _uploadFile.UploadNormalAsync<PhysicalActivityLevel>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }


        await _repo.AddAsync(activity, cancellationToken);

        return await Result<Guid>.SuccessAsync(activity.Id);
    }

}
