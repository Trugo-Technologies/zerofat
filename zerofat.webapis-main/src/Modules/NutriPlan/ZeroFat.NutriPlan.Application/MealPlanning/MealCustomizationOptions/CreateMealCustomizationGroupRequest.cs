using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationOptions;
public class CreateMealCustomizationOptionRequest : ICommand<Result<DefaultIdType>>
{
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

public class CreateMealCustomizationOptionRequestValidator : CustomValidator<CreateMealCustomizationOptionRequest>
{
    public CreateMealCustomizationOptionRequestValidator(IReadRepository<MealCustomizationOption> repository, IStringLocalizer<CreateMealCustomizationOptionRequestValidator> localaizer)
    {


    }
}


public class CreateMealCustomizationOptionRequestHandler(IRepositoryWithEvents<MealCustomizationOption> repository, IFileStorageManager fileStorageManager) : IRequestHandler<CreateMealCustomizationOptionRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<MealCustomizationOption> _repository = repository;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(CreateMealCustomizationOptionRequest request, CancellationToken cancellationToken)
    {
        var part = new MealCustomizationOption
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            PriceAdjustment = request.PriceAdjustment,
            CaloriesAdjustment = request.CaloriesAdjustment,
            FatAdjustment = request.FatAdjustment,
            CarbsAdjustment = request.CarbsAdjustment,
            ProteinAdjustment = request.ProteinAdjustment,
            IsDefault = request.IsDefault,
            IsReplacement = request.IsReplacement,
            ReplacesComponent = request.ReplacesComponent,
            GroupId = request.GroupId,
            IngredientId = request.IngredientId,
            MealId = request.MealId,
            ImageUrl = request.Image == null ? null : await _fileStorageManager.UploadAsync<MealCustomizationOption>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
            //IconUrl = 
        };

        await _repository.AddAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
