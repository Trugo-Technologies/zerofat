using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Core.PhysicalActivityLevels;
public class UpdatePhysicalActivityLevelRequest : ICommand<Result<Guid>>
{
    public Guid? Id { get; set; }
    public IFormFile? Image { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? ExampleEn { get; set; }
    public string? ExampleAr { get; set; }
    public double ActivityValue { get; set; }
    public bool IsActive { get; set; }
}

public class UpdatePhysicalActivityLevelRequestValidator : CustomValidator<UpdatePhysicalActivityLevelRequest>
{
    public UpdatePhysicalActivityLevelRequestValidator(IReadRepository<PhysicalActivityLevel> repository, IStringLocalizer<UpdatePhysicalActivityLevelRequestValidator> localaizer)
    {

        RuleFor(u => u.ActivityValue)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .GreaterThan(1)
           .WithMessage(localaizer["ActivityValue must be more than 1"]);
    }
}

public class UpdatePhysicalActivityLevelRequestHandler(IRepository<PhysicalActivityLevel> repository, IStringLocalizer<UpdatePhysicalActivityLevelRequestHandler> localizer, IFileStorageManager uploadFile) : IRequestHandler<UpdatePhysicalActivityLevelRequest, Result<Guid>>
{
    private readonly IRepository<PhysicalActivityLevel> _repository = repository;
    private readonly IStringLocalizer<UpdatePhysicalActivityLevelRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _uploadFile = uploadFile;


    public async Task<Result<Guid>> Handle(UpdatePhysicalActivityLevelRequest request, CancellationToken cancellationToken)
    {
        var activity = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = activity ?? throw new NotFoundException(_localizer["Activity not found"]);

        activity.ExampleEn = request.ExampleEn;
        activity.ExampleAr = request.ExampleAr;
        activity.DescriptionAr = request.DescriptionAr;
        activity.DescriptionEn = request.DescriptionEn;
        activity.IsActive = request.IsActive;

        if (request.Image != null)
        {
            activity.ImageUrl = await _uploadFile.UploadAsync<PhysicalActivityLevel>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            activity.ImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<PhysicalActivityLevel>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            activity.ImageOptimzeUrl = await _uploadFile.UploadNormalAsync<PhysicalActivityLevel>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        await _repository.UpdateAsync(activity, cancellationToken);

        return await Result<Guid>.SuccessAsync(activity.Id);
    }
}

