using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealPlans;
public class CreateMealPlanRequest : ICommand<Result<DefaultIdType>>
{
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? Code { get; set; }
    public IFormFile? MainImage { get; set; }
    public List<IFormFile>? Images { get; set; }

    public DietitianGoal DefaultDietitianGoal { get; set; }

    public decimal? CarbPercentage { get; set; }
    public decimal? ProteinPercentage { get; set; }
    public decimal? FatPercentage { get; set; }
    public bool IsActive { get; set; }
    public List<MealPlanMealTypeRequest>? MealTypes { get; set; }
}

public class MealPlanMealTypeRequest
{
    public decimal? AverageCalories { get; set; }
    public decimal? Price { get; set; }
    public DefaultIdType MealTypeId { get; set; }
}

public class CreateMealPlanRequestValidator : CustomValidator<CreateMealPlanRequest>
{
    public CreateMealPlanRequestValidator(
        IReadRepository<MealPlan> repository,
        IReadRepository<MealType> mealTypeRepo,
        IStringLocalizer<CreateMealPlanRequestValidator> localizer)
    {

        // Validate NameAr
        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(localizer["Arabic name is required."])
            .MaximumLength(200)
            .WithMessage(localizer["Arabic name must not exceed 200 characters."])
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<MealPlan>(x => x.NameAr == name), _))
            .WithMessage(localizer["Arabic name already exists."]);

        // Validate NameEn
        RuleFor(u => u.NameEn)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(localizer["English name is required."])
            .MaximumLength(200)
            .WithMessage(localizer["English name must not exceed 200 characters."])
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<MealPlan>(x => x.NameEn == name), _))
            .WithMessage(localizer["English name already exists."]);

        // Validate DescriptionAr
        RuleFor(u => u.DescriptionAr)
            .MaximumLength(1000)
            .WithMessage(localizer["Arabic description must not exceed 1000 characters."]);

        // Validate DescriptionEn
        RuleFor(u => u.DescriptionEn)
            .MaximumLength(1000)
            .WithMessage(localizer["English description must not exceed 1000 characters."]);

        // Validate Code
        RuleFor(u => u.Code)
            .NotEmpty()
            .WithMessage(localizer["Code is required."])
            .MaximumLength(50)
            .WithMessage(localizer["Code must not exceed 50 characters."])
            .MustAsync(async (code, _) => !await repository.AnyAsync(new ExpressionSpecification<MealPlan>(x => x.Code == code), _))
            .WithMessage(localizer["Code already exists."]);

        RuleFor(u => u.DefaultDietitianGoal)
            .IsInEnum()
            .WithMessage(localizer["Default dietitian goal is required."]);

        RuleFor(x => x)
             .Must(model => (model.CarbPercentage ?? 0) + (model.ProteinPercentage ?? 0) + (model.FatPercentage ?? 0) == 100)
             .WithMessage(localizer["The sum of CarbPercentage, ProteinPercentage, and FatPercentage must equal 100."])
             .Must(model => model.CarbPercentage is >= 0 and <= 100)
             .WithMessage(localizer["CarbPercentage must be between 0 and 100."])
             .Must(model => model.ProteinPercentage is >= 0 and <= 100)
             .WithMessage(localizer["ProteinPercentage must be between 0 and 100."])
             .Must(model => model.FatPercentage is >= 0 and <= 100)
             .WithMessage(localizer["FatPercentage must be between 0 and 100."]);

        // Validate MealTypes
        RuleFor(u => u.MealTypes)
            .NotNull()
            .WithMessage(localizer["Meal types are required."])
            .Must(mealTypes => mealTypes != null && mealTypes.Count != 0)
            .WithMessage(localizer["At least one meal type is required."]);

        RuleForEach(u => u.MealTypes)
            .ChildRules(mealType =>
            {
                mealType.RuleFor(mt => mt.AverageCalories)
                    .GreaterThan(0)
                    .WithMessage(localizer["Average calories must be greater than 0."]);

                mealType.RuleFor(mt => mt.Price)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage(localizer["Price must be greater than or equal to 0."]);

                mealType.RuleFor(mt => mt.MealTypeId)
                    .NotEmpty()
                        .WithMessage(localizer["Meal type ID is required."])
                    .MustAsync(async (id, _) => await mealTypeRepo.AnyAsync(new ExpressionSpecification<MealType>(x => x.Id == id), _))
                        .WithMessage(localizer["Invaild Meal type ID."]);
            });

    }
}


public class CreateMealPlanRequestHandler(IRepositoryWithEvents<MealPlan> repository, IFileStorageManager fileStorageManager) : ICommandHandler<CreateMealPlanRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<MealPlan> _repository = repository;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(CreateMealPlanRequest request, CancellationToken cancellationToken)
    {
        var mealPlan = new MealPlan
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            Code = request.Code,
            CarbPercentage = request.CarbPercentage,
            FatPercentage = request.FatPercentage,
            DefaultDietitianGoal = request.DefaultDietitianGoal,
            ProteinPercentage = request.ProteinPercentage,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            IsActive = request.IsActive,
            ImageUrl = request.MainImage == null ? null : await _fileStorageManager.UploadAsync<MealPlan>(request.MainImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
        };

        if(request.Images?.Count > 0)
        {
            foreach (var image in request.Images)
            {
                mealPlan.Images.Add(await _fileStorageManager.UploadAsync<MealPlan>(image, FileType.Image, ModuleConstant.ModuleName, cancellationToken));
            }
        }

        if (request.MealTypes?.Count > 0)
        {
            foreach (var mealType in request.MealTypes)
            {
                mealPlan.MealPlanMealTypes.Add(new MealPlanMealType()
                {
                    MealTypeId = mealType.MealTypeId,
                    AverageCalories = mealType.AverageCalories,
                    Price = mealType.Price
                });
            }
        }


        await _repository.AddAsync(mealPlan, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(mealPlan.Id);
    }

}
