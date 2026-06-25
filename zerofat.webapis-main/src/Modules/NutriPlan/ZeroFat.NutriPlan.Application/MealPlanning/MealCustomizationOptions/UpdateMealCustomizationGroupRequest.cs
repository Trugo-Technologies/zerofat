using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationOptions;
public class UpdateMealCustomizationOptionRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public DefaultIdType GroupId { get; set; }
    public DefaultIdType IngredientId { get; set; }

    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public IFormFile? Image { get; set; }
    public decimal PriceAdjustment { get; set; } // Can be positive or negative

    // Nutritional impact
    public double CaloriesAdjustment { get; set; }
    public double FatAdjustment { get; set; }
    public double CarbsAdjustment { get; set; }
    public double ProteinAdjustment { get; set; }
    // Indicates if this is the default selection in its group
    public bool IsDefault { get; set; }
    // For options that replace rather than add to the base (e.g., different protein)
    public bool IsReplacement { get; set; }
    // For replacement options, what they replace (e.g., "Chicken" replaces "Base Protein")
    public string? ReplacesComponent { get; set; }
    public DefaultIdType? MealId { get; set; } // The base meal this option belongs to
}

public class UpdateMealCustomizationOptionRequestValidator : CustomValidator<UpdateMealCustomizationOptionRequest>
{
    public UpdateMealCustomizationOptionRequestValidator(IReadRepository<MealCustomizationOption> repository, IStringLocalizer<UpdateMealCustomizationOptionRequestValidator> localaizer)
    {


    }
}

public class UpdateMealCustomizationOptionRequestHandler(IRepositoryWithEvents<MealCustomizationOption> repository, IFileStorageManager fileStorageManager, IStringLocalizer<UpdateMealCustomizationOptionRequestHandler> localizer) : IRequestHandler<UpdateMealCustomizationOptionRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<MealCustomizationOption> _repository = repository;
    private readonly IStringLocalizer<UpdateMealCustomizationOptionRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(UpdateMealCustomizationOptionRequest request, CancellationToken cancellationToken)
    {
        var option = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = option ?? throw new NotFoundException(_localizer["MealCustomizationOption not found"]);

        option.NameAr = request.NameAr ?? option.NameAr;
        option.NameEn = request.NameEn ?? option.NameEn;
        option.PriceAdjustment = request.PriceAdjustment;
        option.CaloriesAdjustment = request.CaloriesAdjustment;
        option.FatAdjustment = request.FatAdjustment;
        option.CarbsAdjustment = request.CarbsAdjustment;
        option.ProteinAdjustment = request.ProteinAdjustment;
        option.IsDefault = request.IsDefault;
        option.IsReplacement = request.IsReplacement;
        option.ReplacesComponent = request.ReplacesComponent;
        option.GroupId = request.GroupId;
        option.IngredientId = request.IngredientId;
        option.MealId = request.MealId;

        option.ImageUrl = request.Image == null ? option.ImageUrl : await _fileStorageManager.UploadAsync<MealCustomizationOption>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);

        await _repository.UpdateAsync(option, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(option.Id);
    }
}

