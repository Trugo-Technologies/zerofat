using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Ingredients;
public class CreateIngredientRequest : ICommand<Result<DefaultIdType>>
{
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
    public List<string> Tags { get; set; } = [];

    public List<Guid> AllergenIds { get; set; } = [];
    public List<IngredientMeasurementUnitRequest> IngredientMeasurementUnits { get; set; } = [];
    public List<IngredientAttributeRequest> Attributes { get; set; } = [];
}

public class IngredientMeasurementUnitRequest
{
    public string? Code { get; set; }
    public double EquivalentInUnit { get; set; }
}

public class IngredientAttributeRequest
{
    public decimal? Value { get; set; }
    public Guid NutrientsAttributeId { get; set; }
}

public class CreateIngredientRequestValidator : CustomValidator<CreateIngredientRequest>
{
    public CreateIngredientRequestValidator(
        IReadRepository<Ingredient> repository,
        IReadRepository<Category> categoryRepo,
        IReadRepository<Allergen> allergenRepo,
        IStringLocalizer<CreateIngredientRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<Ingredient>(x => x.NameAr == name), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<Ingredient>(x => x.NameEn == name), _))
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


public class CreateIngredientRequestHandler(
    IRepositoryWithEvents<Ingredient> repository) : ICommandHandler<CreateIngredientRequest, Result<Guid>>
{

    public async Task<Result<DefaultIdType>> Handle(CreateIngredientRequest request, CancellationToken cancellationToken)
    {
        var ingredient = new Ingredient
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            StorageInstructionsAr = request.StorageInstructionsAr,
            StorageInstructionsEn = request.StorageInstructionsEn,

            BasicUnit = request.BasicUnit,
            CategoryId = request.CategoryId,
            DietaryPreference = request.DietaryPreference,
            Density = request.Density,
            CostPer100Unit = request.CostPer100Unit,
            IsDairyFree = request.IsDairyFree,
            IsGlutenFree = request.IsGlutenFree,
            IsLowGI = request.IsLowGI,
            IsOrganic = request.IsOrganic,
            IsSeasonal = request.IsSeasonal,

            CaloriesUnit = request.CaloriesUnit,

            CaloriesPer100Unit = request.CaloriesPer100Unit,
            CarbsPer100Unit = request.CarbsPer100Unit,
            FatPer100Unit = request.FatPer100Unit,
            FiberPer100Unit = request.FiberPer100Unit,
            ProteinPer100Unit = request.ProteinPer100Unit,
            SugarPer100Unit = request.SugarPer100Unit,
            WaterPer100g = request.WaterPer100g,

            CholesterolPer100Unit = request.CholesterolPer100Unit,
            TotalSaturatedFattyAcids = request.TotalSaturatedFattyAcids,
            SaturatedFattyAcid4_0 = request.SaturatedFattyAcid4_0,
            SaturatedFattyAcid6_0 = request.SaturatedFattyAcid6_0,
            SaturatedFattyAcid8_0 = request.SaturatedFattyAcid8_0,
            SaturatedFattyAcid10_0 = request.SaturatedFattyAcid10_0,
            SaturatedFattyAcid12_0 = request.SaturatedFattyAcid12_0,
            SaturatedFattyAcid14_0 = request.SaturatedFattyAcid14_0,
            SaturatedFattyAcid16_0 = request.SaturatedFattyAcid16_0,
            SaturatedFattyAcid18_0 = request.SaturatedFattyAcid18_0,

            TotalMonounsaturatedFattyAcids = request.TotalMonounsaturatedFattyAcids,
            MonounsaturatedFattyAcid16_1 = request.MonounsaturatedFattyAcid16_1,
            MonounsaturatedFattyAcid18_1 = request.MonounsaturatedFattyAcid18_1,
            MonounsaturatedFattyAcid20_1 = request.MonounsaturatedFattyAcid20_1,
            MonounsaturatedFattyAcid22_1 = request.MonounsaturatedFattyAcid22_1,

            TotalPolyunsaturatedFattyAcids = request.TotalPolyunsaturatedFattyAcids,
            PolyunsaturatedFattyAcid18_2 = request.PolyunsaturatedFattyAcid18_2,
            PolyunsaturatedFattyAcid18_3 = request.PolyunsaturatedFattyAcid18_3,
            PolyunsaturatedFattyAcid18_4 = request.PolyunsaturatedFattyAcid18_4,
            PolyunsaturatedFattyAcid20_4 = request.PolyunsaturatedFattyAcid20_4,
            PolyunsaturatedFattyAcid20_5 = request.PolyunsaturatedFattyAcid20_5,
            PolyunsaturatedFattyAcid22_5 = request.PolyunsaturatedFattyAcid22_5,
            PolyunsaturatedFattyAcid22_6 = request.PolyunsaturatedFattyAcid22_6,

            CalciumPer100Unit = request.CalciumPer100Unit,
            IronPer100Unit = request.IronPer100Unit,
            MagnesiumPer100Unit = request.MagnesiumPer100Unit,
            PhosphorusPer100Unit = request.PhosphorusPer100Unit,
            PotassiumPer100Unit = request.PotassiumPer100Unit,
            SodiumPer100Unit = request.SodiumPer100Unit,
            ZincPer100Unit = request.ZincPer100Unit,
            CopperPer100Unit = request.CopperPer100Unit,
            SeleniumPer100Unit = request.SeleniumPer100Unit,

            VitaminARAE = request.VitaminARAE,
            Retinol = request.Retinol,
            CaroteneAlpha = request.CaroteneAlpha,
            CaroteneBeta = request.CaroteneBeta,
            CryptoxanthinBeta = request.CryptoxanthinBeta,
            Lycopene = request.Lycopene,
            LuteinZeaxanthin = request.LuteinZeaxanthin,

            VitaminE = request.VitaminE,
            VitaminD = request.VitaminD,
            VitaminK = request.VitaminK,
            VitaminC = request.VitaminC,
            Thiamin = request.Thiamin,
            Riboflavin = request.Riboflavin,
            Niacin = request.Niacin,
            VitaminB6 = request.VitaminB6,
            FolateTotal = request.FolateTotal,
            FolateDFE = request.FolateDFE,
            FolicAcid = request.FolicAcid,
            FolateFood = request.FolateFood,
            VitaminB12 = request.VitaminB12,

            Choline = request.Choline,
            Alcohol = request.Alcohol,
            Caffeine = request.Caffeine,
            Theobromine = request.Theobromine,
            AddedVitaminE = request.AddedVitaminE,
            AddedVitaminB12 = request.AddedVitaminB12,

            Tags = request.Tags,
            Type = request.Type,
            Status = request.Status,
        };

        if (request.AllergenIds?.Count > 0)
        {
            foreach (DefaultIdType id in request.AllergenIds)
            {
                ingredient.IngredientAllergens.Add(new IngredientAllergen
                {
                    AllergenId = id
                });
            }
        }

        if (request.Attributes?.Count > 0)
        {
            foreach (var attribute in request.Attributes)
            {
                ingredient.Attributes.Add(new IngredientAttribute
                {
                    NutrientsAttributeId = attribute.NutrientsAttributeId,
                    Value = attribute.Value,
                });
            }
        }

        ingredient.IngredientMeasurementUnits.Add(new IngredientMeasurementUnit
        {
            Code = ingredient.BasicUnit == BasicUnitType.Liquid ? "ML" : "G",
            EquivalentInUnit = 1,
            IsDefault = true
        });

        if (request.IngredientMeasurementUnits?.Count > 0)
        {
            foreach (IngredientMeasurementUnitRequest ingredientMeasurementUnit in request.IngredientMeasurementUnits)
            {
                ingredient.IngredientMeasurementUnits.Add(new IngredientMeasurementUnit
                {
                    Code = ingredientMeasurementUnit.Code,
                    IsDefault = true,
                    EquivalentInUnit = ingredientMeasurementUnit.EquivalentInUnit
                });
            }
        }

        await repository.AddAsync(ingredient, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(ingredient.Id);
    }

}
