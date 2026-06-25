using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Application.MeasurementUnits;
using ZeroFat.NutriPlan.Application.NutrientsAttributes;
using ZeroFat.NutriPlan.Application.Settings.Allergens;
using ZeroFat.NutriPlan.Application.Settings.Categories;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Ingredients;

public class IngredientSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }

    public string? NameEn { get; set; }
    public string? NameAr { get; set; }

    public BasicUnitType BasicUnit { get; set; }
    public double CaloriesPer100Unit { get; set; }
    public double FatPer100Unit { get; set; }
    public double CarbsPer100Unit { get; set; }
    public double ProteinPer100Unit { get; set; }
    public double FiberPer100Unit { get; set; }
    public double SugarPer100Unit { get; set; }
    public double WaterPer100g { get; set; }

    public List<IngredientMeasurementUnitDto>? IngredientMeasurementUnits { get; set; }
    public double Density { get; set; }
    public BasicUnit CaloriesUnit { get; set; }
    public decimal CostPer100Unit { get; set; }

    public IngredientStatus Status { get; set; }
    public IngredientType Type { get; set; }
    public List<string> Tags { get; set; }
}
public class IngredientRawDto : IngredientSimplifyDto
{
    public DefaultIdType? CategoryId { get; set; }
    public bool IsDairyFree { get; set; }
    public bool IsLowGI { get; set; }
    public bool IsSeasonal { get; set; }
    public bool IsOrganic { get; set; }
    public bool IsGlutenFree { get; set; }
    public DietaryPreference DietaryPreference { get; set; }
}

public class IngredientAuditableDto : IngredientRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class IngredientDto : IngredientAuditableDto
{
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

    public List<AllergenSimplifyDto>? Allergens { get; set; }
    public CategorySimplifyDto? Category { get; set; }

}

public class IngredientDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }
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


    public double Density { get; set; }
    public BasicUnit CaloriesUnit { get; set; }
    public decimal CostPer100Unit { get; set; }

    public IngredientStatus Status { get; set; }
    public IngredientType Type { get; set; }
    public List<string> Tags { get; set; }

    public DefaultIdType? CategoryId { get; set; }
    public bool IsDairyFree { get; set; }
    public bool IsLowGI { get; set; }
    public bool IsSeasonal { get; set; }
    public bool IsOrganic { get; set; }
    public bool IsGlutenFree { get; set; }
    public DietaryPreference DietaryPreference { get; set; }


    public CategorySimplifyDto? Category { get; set; }
    public List<AllergenSimplifyDto>? Allergens { get; set; }
    public List<IngredientMeasurementUnitDto>? IngredientMeasurementUnits { get; set; }
    public List<IngredientAttributeDto>? Attributes { get; set; }
}

public class IngredientAttributeDto : IDto
{
    public decimal? Value { get; set; }
    public Guid NutrientsAttributeId { get; set; }
    public NutrientsAttributeSimplifyDto? NutrientsAttribute { get; set; }
}

public class IngredientMeasurementUnitDto : IDto
{
    public string? Code { get; set; }
    public double EquivalentInUnit { get; set; }
    public bool IsDefault { get; set; }
    public MeasurementUnitSimplifyDto? MeasurementUnit { get; set; }
}
