using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealPlans;
public class UpdateMealPlanRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public IFormFile? MainImage { get; set; }
    public List<IFormFile>? Images { get; set; }

    public DietitianGoal DefaultDietitianGoal { get; set; }

    public decimal? CarbPercentage { get; set; }
    public decimal? ProteinPercentage { get; set; }
    public decimal? FatPercentage { get; set; }
    public bool IsActive { get; set; }
    public List<MealPlanMealTypeRequest>? MealTypes { get; set; }
}

public class UpdateMealPlanRequestValidator : CustomValidator<UpdateMealPlanRequest>
{
    public UpdateMealPlanRequestValidator(
        IReadRepository<MealPlan> repository,
        IReadRepository<MealType> mealTypeRepo,
        IStringLocalizer<UpdateMealPlanRequestValidator> localizer)
    {
        // Validate NameAr
        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(localizer["Arabic name is required."])
            .MaximumLength(200)
            .WithMessage(localizer["Arabic name must not exceed 200 characters."])
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<MealPlan>(x => x.NameAr == name && req.Id != x.Id), _))
            .WithMessage(localizer["Arabic name already exists."]);

        // Validate NameEn
        RuleFor(u => u.NameEn)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(localizer["English name is required."])
            .MaximumLength(200)
            .WithMessage(localizer["English name must not exceed 200 characters."])
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<MealPlan>(x => x.NameEn == name && req.Id != x.Id), _))
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
        // RuleFor(u => u.Code)
        //     .NotEmpty()
        //     .WithMessage(localizer["Code is required."])
        //     .MaximumLength(50)
        //     .WithMessage(localizer["Code must not exceed 50 characters."])
        //     .MustAsync(async (code, _) => !await repository.AnyAsync(new ExpressionSpecification<MealPlan>(x => x.Code == code), _))
        //     .WithMessage(localizer["Code already exists."]);

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

public class UpdateMealPlanRequestHandler(
    IRepositoryWithEvents<MealPlan> repository,
    IRepositoryWithEvents<MealPlanMealType> mealPlanMealTypeRepo,
    IFileStorageManager fileStorageManager,
    IStringLocalizer<UpdateMealPlanRequestHandler> localizer) : IRequestHandler<UpdateMealPlanRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<MealPlan> _repository = repository;
    private readonly IRepositoryWithEvents<MealPlanMealType> _mealPlanMealTypeRepo = mealPlanMealTypeRepo;
    private readonly IStringLocalizer<UpdateMealPlanRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(UpdateMealPlanRequest request, CancellationToken cancellationToken)
    {
        var mealPlan = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);
        _ = mealPlan ?? throw new NotFoundException(_localizer["MealPlan not found"]);

        if (mealPlan.StripeId.IsEmpty())
        {
            mealPlan.NameEn = request.NameEn;
            mealPlan.NameAr = request.NameAr;
        }

        mealPlan.CarbPercentage = request.CarbPercentage;
        mealPlan.FatPercentage = request.FatPercentage;
        mealPlan.ProteinPercentage = request.ProteinPercentage;
        mealPlan.DefaultDietitianGoal = request.DefaultDietitianGoal;
        mealPlan.DescriptionAr = request.DescriptionAr;
        mealPlan.DescriptionEn = request.DescriptionEn;
        mealPlan.IsActive = request.IsActive;
        mealPlan.ImageUrl = request.MainImage == null ? mealPlan.ImageUrl : await _fileStorageManager.UploadAsync<MealPlan>(request.MainImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        if (request.Images?.Count > 0)
        {
            foreach (var image in request.Images)
            {
                mealPlan.Images.Add(await _fileStorageManager.UploadAsync<MealPlan>(image, FileType.Image, ModuleConstant.ModuleName, cancellationToken));
            }
        }

        if (request.MealTypes?.Count > 0)
        {
            await _mealPlanMealTypeRepo.UpdateRelatedEntitiesAsync(
                mealPlan.Id,
                request.MealTypes,
                getRelatedEntityId: mt => mt.MealTypeId, // Extracts ID from TRequest (MealPlanMealTypeRequest)
                getRelatedEntityIdFromEntity: mt => mt.MealTypeId, // Extracts ID from TRelatedEntity (MealPlanMealType)
                updateExistingEntity: (request, existing) =>
                {
                    existing.AverageCalories = request.AverageCalories;
                    existing.Price = request.Price;
                    existing.MealTypeId = request.MealTypeId;
                    return existing;
                },
                createNewEntity: (request, parentId) => new MealPlanMealType
                {
                    MealPlanId = parentId,
                    MealTypeId = request.MealTypeId,
                    AverageCalories = request.AverageCalories,
                    Price = request.Price,
                },
                expressionSpecification: new ExpressionSpecification<MealPlanMealType>(x => x.MealPlanId == mealPlan.Id),
                cancellationToken);
        }



        await _repository.UpdateAsync(mealPlan, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(mealPlan.Id);
    }
}

