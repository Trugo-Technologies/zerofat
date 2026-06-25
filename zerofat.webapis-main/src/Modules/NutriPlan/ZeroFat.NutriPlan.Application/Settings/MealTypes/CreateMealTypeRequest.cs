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

namespace ZeroFat.NutriPlan.Application.Settings.MealTypes;
public class CreateMealTypeRequest : ICommand<Result<DefaultIdType>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? Icon { get; set; }
    public IFormFile? Image { get; set; }
}

public class CreateMealTypeRequestValidator : CustomValidator<CreateMealTypeRequest>
{
    public CreateMealTypeRequestValidator(IReadRepository<MealType> repository, IStringLocalizer<CreateMealTypeRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<MealType>(x => x.NameAr == name), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<MealType>(x => x.NameEn == name), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}


public class CreateMealTypeRequestHandler(IRepositoryWithEvents<MealType> repository, IFileStorageManager fileStorageManager) : ICommandHandler<CreateMealTypeRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<MealType> _repository = repository;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(CreateMealTypeRequest request, CancellationToken cancellationToken)
    {
        var mealType = new MealType
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            IconUrl = request.Icon == null ? null : await _fileStorageManager.UploadAsync<MealType>(request.Icon, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
            ImageUrl = request.Image == null ? null : await _fileStorageManager.UploadAsync<MealType>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
        };

        await _repository.AddAsync(mealType, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(mealType.Id);
    }

}
