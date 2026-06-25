using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Application.Contracts;

namespace ZeroFat.NutriPlan.Infrastructure.Services;

public class NutriPlanExcelReader : INutriPlanExcelReader
{
    public List<Ingredient> ReadIngredientsFromExcel(string filePath)
    {
        var ingredients = new List<Ingredient>();

        using (var workbook = new XLWorkbook(filePath))
        {
            var worksheet = workbook.Worksheet(1); // Assuming data is in the first sheet
            var rows = worksheet.RowsUsed();

            var count = rows.Count();

            foreach (var row in rows)
            {
                // Skip header row
                if (row.RowNumber() <= 4)
                    continue;

                try
                {
                    var ingredient = new Ingredient
                    {
                        Code = row.Cell(1).GetValue<string>(), // Code
                                                               // Map Excel columns to Ingredient properties
                        NameEn = row.Cell(2).GetValue<string>(), // Food Description
                        DescriptionEn = row.Cell(2).GetValue<string>(), // Use the same as NameEn for simplicity
                        CaloriesPer100Unit = row.Cell(6).GetValue<double>(), // Energy (kcal)
                        ProteinPer100Unit = row.Cell(7).GetValue<double>(), // Protein (g)
                        CarbsPer100Unit = row.Cell(8).GetValue<double>(), // Carbohydrate (g)
                        FatPer100Unit = row.Cell(9).GetValue<double>(), // Total Fat (g)
                        SugarPer100Unit = row.Cell(10).GetValue<double>(), // Total Sugars (g)
                        FiberPer100Unit = row.Cell(11).GetValue<double>(), // Dietary Fiber (g)
                        WaterPer100g = row.Cell(12).GetValue<double>(), // Water (g)

                        CholesterolPer100Unit = row.Cell(13).GetValue<double>(), // Cholesterol (mg)
                        TotalSaturatedFattyAcids = row.Cell(14).GetValue<double>(), // Total Saturated Fatty Acids (g)
                        SaturatedFattyAcid4_0 = row.Cell(15).GetValue<double>(), // 4:0 (g)
                        SaturatedFattyAcid6_0 = row.Cell(16).GetValue<double>(), // 6:0 (g)
                        SaturatedFattyAcid8_0 = row.Cell(17).GetValue<double>(), // 8:0 (g)
                        SaturatedFattyAcid10_0 = row.Cell(18).GetValue<double>(), // 10:0 (g)
                        SaturatedFattyAcid12_0 = row.Cell(19).GetValue<double>(), // 12:0 (g)
                        SaturatedFattyAcid14_0 = row.Cell(20).GetValue<double>(), // 14:0 (g)
                        SaturatedFattyAcid16_0 = row.Cell(21).GetValue<double>(), // 16:0 (g)
                        SaturatedFattyAcid18_0 = row.Cell(22).GetValue<double>(), // 18:0 (g)
                        TotalMonounsaturatedFattyAcids = row.Cell(23).GetValue<double>(), // Total Monounsaturated Fatty Acids (g)
                        MonounsaturatedFattyAcid16_1 = row.Cell(24).GetValue<double>(), // 16:1 (g)
                        MonounsaturatedFattyAcid18_1 = row.Cell(25).GetValue<double>(), // 18:1 (g)
                        MonounsaturatedFattyAcid20_1 = row.Cell(26).GetValue<double>(), // 20:1 (g)
                        MonounsaturatedFattyAcid22_1 = row.Cell(27).GetValue<double>(), // 22:1 (g)
                        TotalPolyunsaturatedFattyAcids = row.Cell(28).GetValue<double>(), // Total Polyunsaturated Fatty Acids (g)
                        PolyunsaturatedFattyAcid18_2 = row.Cell(29).GetValue<double>(), // 18:2 (g)
                        PolyunsaturatedFattyAcid18_3 = row.Cell(30).GetValue<double>(), // 18:3 (g)
                        PolyunsaturatedFattyAcid18_4 = row.Cell(31).GetValue<double>(), // 18:4 (g)
                        PolyunsaturatedFattyAcid20_4 = row.Cell(32).GetValue<double>(), // 20:4 (g)
                        PolyunsaturatedFattyAcid20_5 = row.Cell(33).GetValue<double>(), // 20:5 (g)
                        PolyunsaturatedFattyAcid22_5 = row.Cell(34).GetValue<double>(), // 22:5 (g)
                        PolyunsaturatedFattyAcid22_6 = row.Cell(35).GetValue<double>(), // 22:6 (g)
                        CalciumPer100Unit = row.Cell(36).GetValue<double>(), // Calcium (mg)
                       
                        IronPer100Unit = row.Cell(37).GetValue<double>(), // Iron (mg)
                        MagnesiumPer100Unit = row.Cell(38).GetValue<double>(), // Magnesium (mg)
                        PhosphorusPer100Unit = row.Cell(39).GetValue<double>(), // Phosphorus (mg)
                        PotassiumPer100Unit = row.Cell(40).GetValue<double>(), // Potassium (mg)
                        SodiumPer100Unit = row.Cell(41).GetValue<double>(), // Sodium (mg)
                        ZincPer100Unit = row.Cell(42).GetValue<double>(), // Zinc (mg)
                        CopperPer100Unit = row.Cell(43).GetValue<double>(), // Copper (mg)
                        SeleniumPer100Unit = row.Cell(44).GetValue<double>(), // Selenium (mcg)
                        VitaminARAE = row.Cell(45).GetValue<double>(), // Vitamin A, RAE (mcg)
                        Retinol = row.Cell(46).GetValue<double>(), // Retinol (mcg)
                        CaroteneAlpha = row.Cell(47).GetValue<double>(), // Carotene, Alpha (mcg)
                        CaroteneBeta = row.Cell(48).GetValue<double>(), // Carotene, Beta (mcg)
                        CryptoxanthinBeta = row.Cell(49).GetValue<double>(), // Cryptoxanthin, Beta (mcg)
                        Lycopene = row.Cell(50).GetValue<double>(), // Lycopene (mcg)
                        LuteinZeaxanthin = row.Cell(51).GetValue<double>(), // Lutein + Zeaxanthin (mcg)
                        VitaminE = row.Cell(52).GetValue<double>(), // Vitamin E (mg)
                        AddedVitaminE = row.Cell(53).GetValue<double>(),

                        VitaminD = row.Cell(54).GetValue<double>(), // Vitamin D (mcg)
                        VitaminK = row.Cell(55).GetValue<double>(), // Vitamin K (mcg)
                        VitaminC = row.Cell(56).GetValue<double>(), // Vitamin C (mg)
                        Thiamin = row.Cell(57).GetValue<double>(), // Thiamin (mg)
                        Riboflavin = row.Cell(58).GetValue<double>(), // Riboflavin (mg)
                        Niacin = row.Cell(59).GetValue<double>(), // Niacin (mg)
                        VitaminB6 = row.Cell(60).GetValue<double>(), // Vitamin B6 (mg)


                        FolateTotal = row.Cell(61).GetValue<double>(), // Folate, Total (mcg)
                        FolateDFE = row.Cell(62).GetValue<double>(), // Folate, DFE (mcg)
                        FolicAcid = row.Cell(63).GetValue<double>(), // Folic Acid (mcg)
                        FolateFood = row.Cell(64).GetValue<double>(), // Folate, Food (mcg)
                        VitaminB12 = row.Cell(65).GetValue<double>(), // Vitamin B12 (mcg)
                        AddedVitaminB12 = row.Cell(66).GetValue<double>(), // Vitamin B12 (mcg)
                        Choline = row.Cell(67).GetValue<double>(), // Choline (mg)
                        Alcohol = row.Cell(68).GetValue<double>(), // Alcohol (g)

                        Caffeine = row.Cell(69).GetValue<double>(), // Caffeine (mg)
                        Theobromine = row.Cell(70).GetValue<double>(), // Theobromine (mg)

                        BasicUnit = BasicUnitType.Solid,
                        Density = 1,
                        CaloriesUnit = BasicUnit.g,
                        CostPer100Unit = 0,
                        DescriptionAr = string.Empty,
                        NameAr = string.Empty,
                        DietaryPreference = DietaryPreference.None,
                        IngredientSource = IngredientSource.ZeroFat,
                        StorageInstructionsAr = string.Empty,
                        Type = IngredientType.Other,
                        StorageInstructionsEn = string.Empty,
                        Status = IngredientStatus.Available,
                        IsDairyFree = false,
                        IsGlutenFree = false,
                        IsLowGI = false,
                        IsOrganic = false,
                        IsSeasonal = false,
                        IngredientMeasurementUnits =
                        [
                            new ()
                            {
                                EquivalentInUnit = 1,
                                IsDefault = true,
                                Code = "G"
                            }
                        ]
                        // Add other properties as needed
                    };

                    ingredients.Add(ingredient);
                }
                catch (Exception e)
                {

                }
               
            }
        }

        return ingredients;
    }
}
