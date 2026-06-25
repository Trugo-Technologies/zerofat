using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Ingredients;
public class UpdateIngredientRequest : ICommand<Result<Guid>>
{
    public UpdateIngredientRequest()
    {
        Attributes = [];
        IngredientMeasurementUnits = [];
        AllergenIds = [];
        Tags = [];
    }
    public Guid? Id { get; set; }
    public string? NameEn { get; set; }
    public string? DescriptionEn { get; set; }
    public string? StorageInstructionsEn { get; set; }

    public string? NameAr { get; set; }
    public string? DescriptionAr { get; set; }
    public string? StorageInstructionsAr { get; set; }
    public BasicUnitType BasicUnit { get; set; }

    public double CaloriesPer100Unit { get; set; }
    public double FatPer100Unit { get; set; }
    public double CarbsPer100Unit { get; set; }
    public double ProteinPer100Unit { get; set; }
    public double FiberPer100Unit { get; set; }
    public double SugarPer100Unit { get; set; }
    public double WaterPer100g { get; set; }

    // Additional nutritional properties
    public double CholesterolPer100Unit { get; set; }
    public double TotalSaturatedFattyAcids { get; set; }
    public double SaturatedFattyAcid4_0 { get; set; }
#pragma warning disable CA1707 // Identifiers should not contain underscores
    public double SaturatedFattyAcid6_0 { get; set; }
    public double SaturatedFattyAcid8_0 { get; set; }
    public double SaturatedFattyAcid10_0 { get; set; }
    public double SaturatedFattyAcid12_0 { get; set; }
    public double SaturatedFattyAcid14_0 { get; set; }
    public double SaturatedFattyAcid16_0 { get; set; }
    public double SaturatedFattyAcid18_0 { get; set; }
    public double TotalMonounsaturatedFattyAcids { get; set; }
    public double MonounsaturatedFattyAcid16_1 { get; set; }
    public double MonounsaturatedFattyAcid18_1 { get; set; }
    public double MonounsaturatedFattyAcid20_1 { get; set; }
    public double MonounsaturatedFattyAcid22_1 { get; set; }
    public double TotalPolyunsaturatedFattyAcids { get; set; }
    public double PolyunsaturatedFattyAcid18_2 { get; set; }
    public double PolyunsaturatedFattyAcid18_3 { get; set; }
    public double PolyunsaturatedFattyAcid18_4 { get; set; }
    public double PolyunsaturatedFattyAcid20_4 { get; set; }
    public double PolyunsaturatedFattyAcid20_5 { get; set; }
    public double PolyunsaturatedFattyAcid22_5 { get; set; }
    public double PolyunsaturatedFattyAcid22_6 { get; set; }
#pragma warning restore CA1707 // Identifiers should not contain underscores
    public double CalciumPer100Unit { get; set; }
    public double IronPer100Unit { get; set; }
    public double MagnesiumPer100Unit { get; set; }
    public double PhosphorusPer100Unit { get; set; }
    public double PotassiumPer100Unit { get; set; }
    public double SodiumPer100Unit { get; set; }
    public double ZincPer100Unit { get; set; }
    public double CopperPer100Unit { get; set; }
    public double SeleniumPer100Unit { get; set; }
    public double VitaminARAE { get; set; }
    public double Retinol { get; set; }
    public double CaroteneAlpha { get; set; }
    public double CaroteneBeta { get; set; }
    public double CryptoxanthinBeta { get; set; }
    public double Lycopene { get; set; }
    public double LuteinZeaxanthin { get; set; }
    public double VitaminE { get; set; }
    public double VitaminD { get; set; }
    public double VitaminK { get; set; }
    public double VitaminC { get; set; }
    public double Thiamin { get; set; }
    public double Riboflavin { get; set; }
    public double Niacin { get; set; }
    public double VitaminB6 { get; set; }
    public double FolateTotal { get; set; }
    public double FolateDFE { get; set; }
    public double FolicAcid { get; set; }
    public double FolateFood { get; set; }
    public double VitaminB12 { get; set; }
    public double Choline { get; set; }
    public double Alcohol { get; set; }
    public double Caffeine { get; set; }
    public double Theobromine { get; set; }
    public double AddedVitaminE { get; set; }
    public double AddedVitaminB12 { get; set; }

    public bool IsDairyFree { get; set; }
    public bool IsLowGI { get; set; }
    public bool IsSeasonal { get; set; }
    public bool IsOrganic { get; set; }
    public bool IsGlutenFree { get; set; }

    public DefaultIdType? CategoryId { get; set; }
    public double Density { get; set; }
    public BasicUnit CaloriesUnit { get; set; }
    public decimal CostPer100Unit { get; set; }
    public IngredientStatus Status { get; set; }
    public DietaryPreference DietaryPreference { get; set; }
    public IngredientType Type { get; set; }
    public List<string> Tags { get; set; }

    public List<Guid> AllergenIds { get; set; }

    public List<IngredientMeasurementUnitRequest> IngredientMeasurementUnits { get; set; } = [];
    public List<IngredientAttributeRequest> Attributes { get; set; }
}

public class UpdateIngredientRequestValidator : CustomValidator<UpdateIngredientRequest>
{
    public UpdateIngredientRequestValidator(
        IReadRepository<Ingredient> repository,
        IReadRepository<Category> categoryRepo,
        IReadRepository<Allergen> allergenRepo,
        IStringLocalizer<CreateIngredientRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Ingredient>(x => x.NameAr == name && req.Id != x.Id), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Ingredient>(x => x.NameEn == name && req.Id != x.Id), _))
                .WithMessage(localaizer["English name already exists"]);

        RuleFor(u => u.CategoryId)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .MustAsync(async (id, _) => await categoryRepo.AnyAsync(new ExpressionSpecification<Category>(x => x.Id == id && x.CategoryType == CategoryType.Ingredient), _))
               .WithMessage(localaizer["Category not found"]);

        When(x => x.AllergenIds != null, () =>
        {
            RuleForEach(u => u.AllergenIds)
              .Cascade(CascadeMode.Stop)
              .NotEmpty()
              .MustAsync(async (id, _) => await allergenRepo.AnyAsync(new ExpressionSpecification<Allergen>(x => x.Id == id), _))
                   .WithMessage(localaizer["one allergen not found"]);
        });
    }
}

public class UpdateIngredientRequestHandler(
    IRepositoryWithEvents<Ingredient> repository,
    IRepositoryWithEvents<IngredientAllergen> ingredientAllergenRepo,
    IRepositoryWithEvents<IngredientAttribute> ingredientAttributeRepo,
    IRepositoryWithEvents<IngredientMeasurementUnit> ingredientMeasurementUnitRepo,
    IStringLocalizer<UpdateIngredientRequestHandler> localizer) : IRequestHandler<UpdateIngredientRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<Ingredient> _repository = repository;
    private readonly IRepositoryWithEvents<IngredientAllergen> _ingredientAllergenRepo = ingredientAllergenRepo;
    private readonly IRepositoryWithEvents<IngredientAttribute> _ingredientAttributeRepo = ingredientAttributeRepo;
    private readonly IRepositoryWithEvents<IngredientMeasurementUnit> _ingredientMeasurementUnitRepo = ingredientMeasurementUnitRepo;
    private readonly IStringLocalizer<UpdateIngredientRequestHandler> _localizer = localizer;

    public async Task<Result<Guid>> Handle(UpdateIngredientRequest request, CancellationToken cancellationToken)
    {
        Ingredient? ingredient = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);
        _ = ingredient ?? throw new NotFoundException(_localizer["Ingredient not found"]);

        ingredient.NameAr = request.NameAr;
        ingredient.NameEn = request.NameEn;

        ingredient.DescriptionEn = request.DescriptionEn;
        ingredient.DescriptionAr = request.DescriptionAr;

        ingredient.StorageInstructionsAr = request.StorageInstructionsAr;
        ingredient.StorageInstructionsEn = request.StorageInstructionsEn;

        ingredient.CategoryId = request.CategoryId;
        ingredient.BasicUnit = request.BasicUnit;
        ingredient.IsDairyFree = request.IsDairyFree;
        ingredient.IsGlutenFree = request.IsGlutenFree;
        ingredient.IsLowGI = request.IsLowGI;
        ingredient.IsOrganic = request.IsOrganic;
        ingredient.IsSeasonal = request.IsSeasonal;
        ingredient.DietaryPreference = request.DietaryPreference;
        ingredient.CostPer100Unit = request.CostPer100Unit;
        ingredient.Density = request.Density;

        ingredient.CaloriesUnit = request.CaloriesUnit;

        ingredient.CaloriesPer100Unit = request.CaloriesPer100Unit;
        ingredient.CarbsPer100Unit = request.CarbsPer100Unit;
        ingredient.FatPer100Unit = request.FatPer100Unit;
        ingredient.FiberPer100Unit = request.FiberPer100Unit;
        ingredient.ProteinPer100Unit = request.ProteinPer100Unit;
        ingredient.SugarPer100Unit = request.SugarPer100Unit;
        ingredient.WaterPer100g = request.WaterPer100g;

        ingredient.CholesterolPer100Unit = request.CholesterolPer100Unit;
        ingredient.TotalSaturatedFattyAcids = request.TotalSaturatedFattyAcids;
        ingredient.SaturatedFattyAcid4_0 = request.SaturatedFattyAcid4_0;
        ingredient.SaturatedFattyAcid6_0 = request.SaturatedFattyAcid6_0;
        ingredient.SaturatedFattyAcid8_0 = request.SaturatedFattyAcid8_0;
        ingredient.SaturatedFattyAcid10_0 = request.SaturatedFattyAcid10_0;
        ingredient.SaturatedFattyAcid12_0 = request.SaturatedFattyAcid12_0;
        ingredient.SaturatedFattyAcid14_0 = request.SaturatedFattyAcid14_0;
        ingredient.SaturatedFattyAcid16_0 = request.SaturatedFattyAcid16_0;
        ingredient.SaturatedFattyAcid18_0 = request.SaturatedFattyAcid18_0;

        ingredient.TotalMonounsaturatedFattyAcids = request.TotalMonounsaturatedFattyAcids;
        ingredient.MonounsaturatedFattyAcid16_1 = request.MonounsaturatedFattyAcid16_1;
        ingredient.MonounsaturatedFattyAcid18_1 = request.MonounsaturatedFattyAcid18_1;
        ingredient.MonounsaturatedFattyAcid20_1 = request.MonounsaturatedFattyAcid20_1;
        ingredient.MonounsaturatedFattyAcid22_1 = request.MonounsaturatedFattyAcid22_1;

        ingredient.TotalPolyunsaturatedFattyAcids = request.TotalPolyunsaturatedFattyAcids;
        ingredient.PolyunsaturatedFattyAcid18_2 = request.PolyunsaturatedFattyAcid18_2;
        ingredient.PolyunsaturatedFattyAcid18_3 = request.PolyunsaturatedFattyAcid18_3;
        ingredient.PolyunsaturatedFattyAcid18_4 = request.PolyunsaturatedFattyAcid18_4;
        ingredient.PolyunsaturatedFattyAcid20_4 = request.PolyunsaturatedFattyAcid20_4;
        ingredient.PolyunsaturatedFattyAcid20_5 = request.PolyunsaturatedFattyAcid20_5;
        ingredient.PolyunsaturatedFattyAcid22_5 = request.PolyunsaturatedFattyAcid22_5;
        ingredient.PolyunsaturatedFattyAcid22_6 = request.PolyunsaturatedFattyAcid22_6;

        ingredient.CalciumPer100Unit = request.CalciumPer100Unit;
        ingredient.IronPer100Unit = request.IronPer100Unit;
        ingredient.MagnesiumPer100Unit = request.MagnesiumPer100Unit;
        ingredient.PhosphorusPer100Unit = request.PhosphorusPer100Unit;
        ingredient.PotassiumPer100Unit = request.PotassiumPer100Unit;
        ingredient.SodiumPer100Unit = request.SodiumPer100Unit;
        ingredient.ZincPer100Unit = request.ZincPer100Unit;
        ingredient.CopperPer100Unit = request.CopperPer100Unit;
        ingredient.SeleniumPer100Unit = request.SeleniumPer100Unit;

        ingredient.VitaminARAE = request.VitaminARAE;
        ingredient.Retinol = request.Retinol;
        ingredient.CaroteneAlpha = request.CaroteneAlpha;
        ingredient.CaroteneBeta = request.CaroteneBeta;
        ingredient.CryptoxanthinBeta = request.CryptoxanthinBeta;
        ingredient.Lycopene = request.Lycopene;
        ingredient.LuteinZeaxanthin = request.LuteinZeaxanthin;

        ingredient.VitaminE = request.VitaminE;
        ingredient.VitaminD = request.VitaminD;
        ingredient.VitaminK = request.VitaminK;
        ingredient.VitaminC = request.VitaminC;
        ingredient.Thiamin = request.Thiamin;
        ingredient.Riboflavin = request.Riboflavin;
        ingredient.Niacin = request.Niacin;
        ingredient.VitaminB6 = request.VitaminB6;
        ingredient.FolateTotal = request.FolateTotal;
        ingredient.FolateDFE = request.FolateDFE;
        ingredient.FolicAcid = request.FolicAcid;
        ingredient.FolateFood = request.FolateFood;
        ingredient.VitaminB12 = request.VitaminB12;

        ingredient.Choline = request.Choline;
        ingredient.Alcohol = request.Alcohol;
        ingredient.Caffeine = request.Caffeine;
        ingredient.Theobromine = request.Theobromine;
        ingredient.AddedVitaminE = request.AddedVitaminE;
        ingredient.AddedVitaminB12 = request.AddedVitaminB12;

        ingredient.Tags = request.Tags;
        ingredient.Type = request.Type;
        ingredient.Status = request.Status;

        await _ingredientAllergenRepo.SyncRelationAsync(
               new ExpressionSpecification<IngredientAllergen>(x => x.IngredientId == ingredient.Id),
               request.AllergenIds,
               x => x.AllergenId.Value,
               id => new IngredientAllergen { IngredientId = ingredient.Id, AllergenId = id },
               cancellationToken);

        if (request.Attributes?.Count > 0)
        {
            await _ingredientAttributeRepo.UpdateRelatedEntitiesAsync(
               ingredient.Id,
               request.Attributes,
               getRelatedEntityId: mt => mt.NutrientsAttributeId, // Extracts ID from TRequest (MealPlanMealTypeRequest)
               getRelatedEntityIdFromEntity: mt => mt.NutrientsAttributeId, // Extracts ID from TRelatedEntity (MealPlanMealType)
               updateExistingEntity: (request, existing) =>
               {
                   existing.NutrientsAttributeId = request.NutrientsAttributeId;
                   existing.Value = request.Value;

                   return existing;
               },
               createNewEntity: (request, parentId) => new IngredientAttribute
               {
                   NutrientsAttributeId = request.NutrientsAttributeId,
                   Value = request.Value,
                   IngredientId = ingredient.Id,
               },
               expressionSpecification: new ExpressionSpecification<IngredientAttribute>(x => x.IngredientId == ingredient.Id),
               cancellationToken);

            // List<IngredientAttribute> ingredientAttributes = await _ingredientAttributeRepo.ListAsync(new ExpressionSpecification<IngredientAttribute>(x => x.IngredientId == request.Id), cancellationToken);
            // await _ingredientAttributeRepo.DeleteRangeAsync(ingredientAttributes, withSaveChanges: false, cancellationToken: cancellationToken);
            // List<IngredientAttribute> newItems = [.. request.Attributes.ConvertAll(x => new IngredientAttribute()
            // {
            //     NutrientsAttributeId = x.NutrientsAttributeId,
            //     Value = x.Value,
            //     IngredientId = request.Id.Value,
            // })];
            // 
            // await _ingredientAttributeRepo.AddRangeAsync(newItems, withSaveChanges: false, cancellationToken: cancellationToken);
        }

        if (request.IngredientMeasurementUnits?.Count > 0)
        {
            List<IngredientMeasurementUnit> ingredientMeasurementUnits = await _ingredientMeasurementUnitRepo.ListAsync(new ExpressionSpecification<IngredientMeasurementUnit>(x => x.IngredientId == request.Id), cancellationToken);
            await _ingredientMeasurementUnitRepo.DeleteRangeAsync(ingredientMeasurementUnits, withSaveChanges: false, cancellationToken: cancellationToken);
            List<IngredientMeasurementUnit> newItems =
            [
                new IngredientMeasurementUnit
                {
                    Code = ingredient.BasicUnit == BasicUnitType.Liquid ? "ML" : "G",
                    EquivalentInUnit = 1,
                    IsDefault = true,
                    IngredientId = request.Id,
                },
                .. request.IngredientMeasurementUnits.ConvertAll(x => new IngredientMeasurementUnit()
                {
                    Code = x.Code,
                    IsDefault = false,
                    EquivalentInUnit = x.EquivalentInUnit,
                    IngredientId = request.Id,
                }),
            ];

            await _ingredientMeasurementUnitRepo.AddRangeAsync(newItems, withSaveChanges: false, cancellationToken: cancellationToken);
        }


        await _repository.UpdateAsync(ingredient, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<Guid>.SuccessAsync(ingredient.Id);
    }
}

