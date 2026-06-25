using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.Categories;
public class UpdateCategoryRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? Icon { get; set; }
    public IFormFile? Image { get; set; }
}

public class UpdateCategoryRequestValidator : CustomValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator(IReadRepository<Category> repository, IStringLocalizer<UpdateCategoryRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Category>(x => x.NameAr == name && req.Id != x.Id), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Category>(x => x.NameEn == name && req.Id != x.Id), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}

public class UpdateCategoryRequestHandler(
    IRepositoryWithEvents<Category> repository,
    IFileStorageManager fileStorageManager,
    IStringLocalizer<UpdateCategoryRequestHandler> localizer) : IRequestHandler<UpdateCategoryRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Category> _repository = repository;
    private readonly IStringLocalizer<UpdateCategoryRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["Category not found"]);

        part.NameAr = request.NameAr;
        part.NameEn = request.NameEn;
        part.IconUrl = request.Icon == null ? part.IconUrl : await _fileStorageManager.UploadAsync<Category>(request.Icon, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        part.ImageUrl = request.Image == null ? part.ImageUrl : await _fileStorageManager.UploadAsync<Category>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);

        await _repository.UpdateAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }
}

