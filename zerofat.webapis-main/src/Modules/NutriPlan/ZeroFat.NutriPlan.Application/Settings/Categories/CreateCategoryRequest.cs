using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.Categories;
public class CreateCategoryRequest : ICommand<Result<DefaultIdType>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public CategoryType CategoryType { get; set; }
    public IFormFile? Icon { get; set; }
    public IFormFile? Image { get; set; }
}

public class CreateCategoryRequestValidator : CustomValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator(IReadRepository<Category> repository, IStringLocalizer<CreateCategoryRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<Category>(x => x.NameAr == name), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<Category>(x => x.NameEn == name), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}


public class CreateCategoryRequestHandler(IRepositoryWithEvents<Category> repository, IFileStorageManager fileStorageManager) : ICommandHandler<CreateCategoryRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Category> _repository = repository;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var part = new Category
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            CategoryType = request.CategoryType,
            IconUrl = request.Icon == null ? null : await _fileStorageManager.UploadAsync<Category>(request.Icon, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
            ImageUrl = request.Image == null ? null : await _fileStorageManager.UploadAsync<Category>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken),

            //IconUrl = 
        };

        await _repository.AddAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
