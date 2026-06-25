using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Domain.MealPlanning;
public class Ingredient : AuditableEntity, IAggregateRoot
{
    public Ingredient()
    {
        IngredientAllergens = [];
        IngredientMeasurementUnits = [];
        RecipeIngredients = [];
        Attributes = [];
        Tags = [];
    }

    public string? Code { get; set; }

    /// <summary>
    /// Name of the ingredient (e.g., "Tomato").
    /// </summary>
    public string? NameEn { get; set; }
    /// <summary>
    /// Description or details about the ingredient.
    /// </summary>
    public string? DescriptionEn { get; set; }
    public string? StorageInstructionsEn { get; set; }

    public string? NameAr { get; set; }
    public string? DescriptionAr { get; set; }
    public string? StorageInstructionsAr { get; set; }

    /// <summary>
    /// Enum indicating the basic unit type for the ingredient (e.g., "Solid product (g/kg)").
    /// </summary>
    public BasicUnitType BasicUnit { get; set; }

    /// <summary>
    /// Calories per 100 grams of the ingredient.
    /// </summary>
    public double CaloriesPer100Unit { get; set; }
    /// <summary>
    /// Fat content per 100 grams of the ingredient.
    /// </summary>
    public double FatPer100Unit { get; set; }
    /// <summary>
    /// Carbohydrate content per 100 grams of the ingredient.
    /// </summary>
    public double CarbsPer100Unit { get; set; }
    /// <summary>
    /// Protein content per 100 grams of the ingredient.
    /// </summary>
    public double ProteinPer100Unit { get; set; }
    /// <summary>
    /// Fiber content per 100 grams of the ingredient.
    /// </summary>
    public double FiberPer100Unit { get; set; }
    /// <summary>
    /// Sugar content per 100 grams of the ingredient.
    /// </summary>
    public double SugarPer100Unit { get; set; }
    /// <summary>
    /// Water content per 100 grams of the ingredient.
    /// </summary>
    public double WaterPer100g { get; set; }

    public bool IsDairyFree { get; set; }
    public bool IsLowGI { get; set; }
    public bool IsSeasonal { get; set; }
    /// <summary>
    /// Indicates if the ingredient is organic.
    /// </summary>
    public bool IsOrganic { get; set; }
    /// <summary>
    /// Indicates if the ingredient is gluten-free.
    /// </summary>
    public bool IsGlutenFree { get; set; }

    public DefaultIdType? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    public double Density { get; set; }
    public BasicUnit CaloriesUnit { get; set; }
    public decimal CostPer100Unit { get; set; }

    /// <summary>
    /// Status of the ingredient (e.g., Available, Out of Stock).
    /// </summary>
    public IngredientStatus Status { get; set; }
    public IngredientSource IngredientSource { get; set; }

    public virtual List<IngredientAllergen> IngredientAllergens { get; set; }
    public virtual List<RecipeIngredient> RecipeIngredients { get; set; }
    public virtual List<IngredientMeasurementUnit> IngredientMeasurementUnits { get; set; }
    public virtual List<IngredientAttribute> Attributes { get; set; }

    /// <summary>
    /// Dietary preference (e.g., Vegan, Vegetarian).
    /// </summary>
    public DietaryPreference DietaryPreference { get; set; }

    /// <summary>
    /// The type of ingredient (e.g., Vegetable, Fruit).
    /// </summary>
    public IngredientType Type { get; set; }

    /// <summary>
    /// Tags associated with the ingredient.
    /// </summary>
    public List<string> Tags { get; set; }


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
}
