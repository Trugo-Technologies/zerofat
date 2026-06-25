using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.NutriPlan
{
    /// <inheritdoc />
    public partial class initNutriPlanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "NutriPlan");

            migrationBuilder.CreateTable(
                name: "Allergens",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allergens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    CategoryType = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealCustomizationGroups",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    MinSelection = table.Column<int>(type: "integer", nullable: false),
                    MaxSelection = table.Column<int>(type: "integer", nullable: true),
                    CaloriesAdjustment = table.Column<double>(type: "double precision", nullable: false),
                    FatAdjustment = table.Column<double>(type: "double precision", nullable: false),
                    CarbsAdjustment = table.Column<double>(type: "double precision", nullable: false),
                    ProteinAdjustment = table.Column<double>(type: "double precision", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealCustomizationGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealPlans",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    DescriptionEn = table.Column<string>(type: "text", nullable: true),
                    DescriptionAr = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Images = table.Column<List<string>>(type: "text[]", nullable: false),
                    DefaultDietitianGoal = table.Column<int>(type: "integer", nullable: false),
                    CarbPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    ProteinPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    FatPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    StripeId = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "text", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealTypes",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    Index = table.Column<int>(type: "integer", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    ScheduledTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "text", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementUnits",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NutrientsAttributes",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutrientsAttributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    DescriptionEn = table.Column<string>(type: "text", nullable: true),
                    StorageInstructionsEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    DescriptionAr = table.Column<string>(type: "text", nullable: true),
                    StorageInstructionsAr = table.Column<string>(type: "text", nullable: true),
                    BasicUnit = table.Column<int>(type: "integer", nullable: false),
                    CaloriesPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    FatPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    CarbsPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    ProteinPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    FiberPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    SugarPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    WaterPer100g = table.Column<double>(type: "double precision", nullable: false),
                    IsDairyFree = table.Column<bool>(type: "boolean", nullable: false),
                    IsLowGI = table.Column<bool>(type: "boolean", nullable: false),
                    IsSeasonal = table.Column<bool>(type: "boolean", nullable: false),
                    IsOrganic = table.Column<bool>(type: "boolean", nullable: false),
                    IsGlutenFree = table.Column<bool>(type: "boolean", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Density = table.Column<double>(type: "double precision", nullable: false),
                    CaloriesUnit = table.Column<int>(type: "integer", nullable: false),
                    CostPer100Unit = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IngredientSource = table.Column<int>(type: "integer", nullable: false),
                    DietaryPreference = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    CholesterolPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    TotalSaturatedFattyAcids = table.Column<double>(type: "double precision", nullable: false),
                    SaturatedFattyAcid4_0 = table.Column<double>(type: "double precision", nullable: false),
                    SaturatedFattyAcid6_0 = table.Column<double>(type: "double precision", nullable: false),
                    SaturatedFattyAcid8_0 = table.Column<double>(type: "double precision", nullable: false),
                    SaturatedFattyAcid10_0 = table.Column<double>(type: "double precision", nullable: false),
                    SaturatedFattyAcid12_0 = table.Column<double>(type: "double precision", nullable: false),
                    SaturatedFattyAcid14_0 = table.Column<double>(type: "double precision", nullable: false),
                    SaturatedFattyAcid16_0 = table.Column<double>(type: "double precision", nullable: false),
                    SaturatedFattyAcid18_0 = table.Column<double>(type: "double precision", nullable: false),
                    TotalMonounsaturatedFattyAcids = table.Column<double>(type: "double precision", nullable: false),
                    MonounsaturatedFattyAcid16_1 = table.Column<double>(type: "double precision", nullable: false),
                    MonounsaturatedFattyAcid18_1 = table.Column<double>(type: "double precision", nullable: false),
                    MonounsaturatedFattyAcid20_1 = table.Column<double>(type: "double precision", nullable: false),
                    MonounsaturatedFattyAcid22_1 = table.Column<double>(type: "double precision", nullable: false),
                    TotalPolyunsaturatedFattyAcids = table.Column<double>(type: "double precision", nullable: false),
                    PolyunsaturatedFattyAcid18_2 = table.Column<double>(type: "double precision", nullable: false),
                    PolyunsaturatedFattyAcid18_3 = table.Column<double>(type: "double precision", nullable: false),
                    PolyunsaturatedFattyAcid18_4 = table.Column<double>(type: "double precision", nullable: false),
                    PolyunsaturatedFattyAcid20_4 = table.Column<double>(type: "double precision", nullable: false),
                    PolyunsaturatedFattyAcid20_5 = table.Column<double>(type: "double precision", nullable: false),
                    PolyunsaturatedFattyAcid22_5 = table.Column<double>(type: "double precision", nullable: false),
                    PolyunsaturatedFattyAcid22_6 = table.Column<double>(type: "double precision", nullable: false),
                    CalciumPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    IronPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    MagnesiumPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    PhosphorusPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    PotassiumPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    SodiumPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    ZincPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    CopperPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    SeleniumPer100Unit = table.Column<double>(type: "double precision", nullable: false),
                    VitaminARAE = table.Column<double>(type: "double precision", nullable: false),
                    Retinol = table.Column<double>(type: "double precision", nullable: false),
                    CaroteneAlpha = table.Column<double>(type: "double precision", nullable: false),
                    CaroteneBeta = table.Column<double>(type: "double precision", nullable: false),
                    CryptoxanthinBeta = table.Column<double>(type: "double precision", nullable: false),
                    Lycopene = table.Column<double>(type: "double precision", nullable: false),
                    LuteinZeaxanthin = table.Column<double>(type: "double precision", nullable: false),
                    VitaminE = table.Column<double>(type: "double precision", nullable: false),
                    VitaminD = table.Column<double>(type: "double precision", nullable: false),
                    VitaminK = table.Column<double>(type: "double precision", nullable: false),
                    VitaminC = table.Column<double>(type: "double precision", nullable: false),
                    Thiamin = table.Column<double>(type: "double precision", nullable: false),
                    Riboflavin = table.Column<double>(type: "double precision", nullable: false),
                    Niacin = table.Column<double>(type: "double precision", nullable: false),
                    VitaminB6 = table.Column<double>(type: "double precision", nullable: false),
                    FolateTotal = table.Column<double>(type: "double precision", nullable: false),
                    FolateDFE = table.Column<double>(type: "double precision", nullable: false),
                    FolicAcid = table.Column<double>(type: "double precision", nullable: false),
                    FolateFood = table.Column<double>(type: "double precision", nullable: false),
                    VitaminB12 = table.Column<double>(type: "double precision", nullable: false),
                    Choline = table.Column<double>(type: "double precision", nullable: false),
                    Alcohol = table.Column<double>(type: "double precision", nullable: false),
                    Caffeine = table.Column<double>(type: "double precision", nullable: false),
                    Theobromine = table.Column<double>(type: "double precision", nullable: false),
                    AddedVitaminE = table.Column<double>(type: "double precision", nullable: false),
                    AddedVitaminB12 = table.Column<double>(type: "double precision", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredients_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "NutriPlan",
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    PreparationMethodEn = table.Column<string>(type: "text", nullable: true),
                    PreparationMethodAr = table.Column<string>(type: "text", nullable: true),
                    FullRecipeTextEn = table.Column<string>(type: "text", nullable: true),
                    FullRecipeTextAr = table.Column<string>(type: "text", nullable: true),
                    PreparationTime = table.Column<int>(type: "integer", nullable: false),
                    CookingTime = table.Column<int>(type: "integer", nullable: false),
                    Servings = table.Column<int>(type: "integer", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    Cuisine = table.Column<int>(type: "integer", nullable: true),
                    IsGlutenFree = table.Column<bool>(type: "boolean", nullable: false),
                    IsDairyFree = table.Column<bool>(type: "boolean", nullable: false),
                    IsLowGI = table.Column<bool>(type: "boolean", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsCold = table.Column<bool>(type: "boolean", nullable: false),
                    IsWarm = table.Column<bool>(type: "boolean", nullable: false),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    WeightInGrams = table.Column<double>(type: "double precision", nullable: false),
                    Calories = table.Column<double>(type: "double precision", nullable: false),
                    Fat = table.Column<double>(type: "double precision", nullable: false),
                    Carbs = table.Column<double>(type: "double precision", nullable: false),
                    Protein = table.Column<double>(type: "double precision", nullable: false),
                    Water = table.Column<double>(type: "double precision", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    DietaryCategories = table.Column<int[]>(type: "integer[]", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "NutriPlan",
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MealPlanMealTypes",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AverageCalories = table.Column<decimal>(type: "numeric", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    MealTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    MealPlanId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlanMealTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealPlanMealTypes_MealPlans_MealPlanId",
                        column: x => x.MealPlanId,
                        principalSchema: "NutriPlan",
                        principalTable: "MealPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealPlanMealTypes_MealTypes_MealTypeId",
                        column: x => x.MealTypeId,
                        principalSchema: "NutriPlan",
                        principalTable: "MealTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyMenus",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    AverageCalories = table.Column<decimal>(type: "numeric", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    MealPlanId = table.Column<Guid>(type: "uuid", nullable: true),
                    MealTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    MenuId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyMenus_MealPlans_MealPlanId",
                        column: x => x.MealPlanId,
                        principalSchema: "NutriPlan",
                        principalTable: "MealPlans",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DailyMenus_MealTypes_MealTypeId",
                        column: x => x.MealTypeId,
                        principalSchema: "NutriPlan",
                        principalTable: "MealTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DailyMenus_Menus_MenuId",
                        column: x => x.MenuId,
                        principalSchema: "NutriPlan",
                        principalTable: "Menus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IngredientAllergens",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uuid", nullable: true),
                    AllergenId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientAllergens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientAllergens_Allergens_AllergenId",
                        column: x => x.AllergenId,
                        principalSchema: "NutriPlan",
                        principalTable: "Allergens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IngredientAllergens_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalSchema: "NutriPlan",
                        principalTable: "Ingredients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IngredientAttributes",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: true),
                    IngredientId = table.Column<Guid>(type: "uuid", nullable: false),
                    NutrientsAttributeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientAttributes_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalSchema: "NutriPlan",
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientAttributes_NutrientsAttributes_NutrientsAttribute~",
                        column: x => x.NutrientsAttributeId,
                        principalSchema: "NutriPlan",
                        principalTable: "NutrientsAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IngredientMeasurementUnits",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    EquivalentInUnit = table.Column<double>(type: "double precision", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    MeasurementUnitId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientMeasurementUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientMeasurementUnits_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalSchema: "NutriPlan",
                        principalTable: "Ingredients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IngredientMeasurementUnits_MeasurementUnits_MeasurementUnit~",
                        column: x => x.MeasurementUnitId,
                        principalSchema: "NutriPlan",
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    PreparationMethodEn = table.Column<string>(type: "text", nullable: true),
                    PreparationMethodAr = table.Column<string>(type: "text", nullable: true),
                    FullRecipeTextEn = table.Column<string>(type: "text", nullable: true),
                    FullRecipeTextAr = table.Column<string>(type: "text", nullable: true),
                    DietaryCategories = table.Column<int[]>(type: "integer[]", nullable: false),
                    WeightInGrams = table.Column<double>(type: "double precision", nullable: false),
                    Calories = table.Column<double>(type: "double precision", nullable: false),
                    Fat = table.Column<double>(type: "double precision", nullable: false),
                    Carbs = table.Column<double>(type: "double precision", nullable: false),
                    Protein = table.Column<double>(type: "double precision", nullable: false),
                    Water = table.Column<double>(type: "double precision", nullable: false),
                    Cuisine = table.Column<int>(type: "integer", nullable: true),
                    IsAddOn = table.Column<bool>(type: "boolean", nullable: false),
                    PriceForCustomer = table.Column<double>(type: "double precision", nullable: false),
                    IsGlutenFree = table.Column<bool>(type: "boolean", nullable: false),
                    IsDairyFree = table.Column<bool>(type: "boolean", nullable: false),
                    IsLowGI = table.Column<bool>(type: "boolean", nullable: false),
                    IsCold = table.Column<bool>(type: "boolean", nullable: false),
                    IsWarm = table.Column<bool>(type: "boolean", nullable: false),
                    SuitableForFreezing = table.Column<bool>(type: "boolean", nullable: false),
                    OrginalMealId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "text", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meals_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalSchema: "NutriPlan",
                        principalTable: "Recipes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RecipeIngredients",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HideOnCustomerPorter = table.Column<bool>(type: "boolean", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    WeightInGrams = table.Column<double>(type: "double precision", nullable: false),
                    IsOptional = table.Column<bool>(type: "boolean", nullable: false),
                    BasicUnit = table.Column<int>(type: "integer", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasurementUnitCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalSchema: "NutriPlan",
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalSchema: "NutriPlan",
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeMealTypes",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MealTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeMealTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeMealTypes_MealTypes_MealTypeId",
                        column: x => x.MealTypeId,
                        principalSchema: "NutriPlan",
                        principalTable: "MealTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecipeMealTypes_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalSchema: "NutriPlan",
                        principalTable: "Recipes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomMeals",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    SpecialInstructions = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    BaseMealId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalCalories = table.Column<double>(type: "double precision", nullable: false),
                    TotalFat = table.Column<double>(type: "double precision", nullable: false),
                    TotalCarbs = table.Column<double>(type: "double precision", nullable: false),
                    TotalProtein = table.Column<double>(type: "double precision", nullable: false),
                    TotalPrice = table.Column<double>(type: "double precision", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomMeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomMeals_Meals_BaseMealId",
                        column: x => x.BaseMealId,
                        principalSchema: "NutriPlan",
                        principalTable: "Meals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DailyMenuMeals",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    MealId = table.Column<Guid>(type: "uuid", nullable: true),
                    DailyMenuId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyMenuMeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyMenuMeals_DailyMenus_DailyMenuId",
                        column: x => x.DailyMenuId,
                        principalSchema: "NutriPlan",
                        principalTable: "DailyMenus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DailyMenuMeals_Meals_MealId",
                        column: x => x.MealId,
                        principalSchema: "NutriPlan",
                        principalTable: "Meals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Extras",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    WeightInGrams = table.Column<double>(type: "double precision", nullable: false),
                    OrginalIngredientId = table.Column<Guid>(type: "uuid", nullable: true),
                    IngredientId = table.Column<Guid>(type: "uuid", nullable: true),
                    MealId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Extras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Extras_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalSchema: "NutriPlan",
                        principalTable: "Ingredients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Extras_Meals_MealId",
                        column: x => x.MealId,
                        principalSchema: "NutriPlan",
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealAllergens",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MealId = table.Column<Guid>(type: "uuid", nullable: true),
                    AllergenId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealAllergens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealAllergens_Allergens_AllergenId",
                        column: x => x.AllergenId,
                        principalSchema: "NutriPlan",
                        principalTable: "Allergens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MealAllergens_Meals_MealId",
                        column: x => x.MealId,
                        principalSchema: "NutriPlan",
                        principalTable: "Meals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MealCustomizationOptions",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    PriceAdjustment = table.Column<decimal>(type: "numeric", nullable: false),
                    CaloriesAdjustment = table.Column<double>(type: "double precision", nullable: false),
                    FatAdjustment = table.Column<double>(type: "double precision", nullable: false),
                    CarbsAdjustment = table.Column<double>(type: "double precision", nullable: false),
                    ProteinAdjustment = table.Column<double>(type: "double precision", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsReplacement = table.Column<bool>(type: "boolean", nullable: false),
                    ReplacesComponent = table.Column<string>(type: "text", nullable: true),
                    MealId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealCustomizationOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealCustomizationOptions_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalSchema: "NutriPlan",
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealCustomizationOptions_MealCustomizationGroups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "NutriPlan",
                        principalTable: "MealCustomizationGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealCustomizationOptions_Meals_MealId",
                        column: x => x.MealId,
                        principalSchema: "NutriPlan",
                        principalTable: "Meals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomMealOptions",
                schema: "NutriPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomMealId = table.Column<Guid>(type: "uuid", nullable: false),
                    OptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomMealOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomMealOptions_CustomMeals_CustomMealId",
                        column: x => x.CustomMealId,
                        principalSchema: "NutriPlan",
                        principalTable: "CustomMeals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomMealOptions_MealCustomizationOptions_OptionId",
                        column: x => x.OptionId,
                        principalSchema: "NutriPlan",
                        principalTable: "MealCustomizationOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomMealOptions_CustomMealId",
                schema: "NutriPlan",
                table: "CustomMealOptions",
                column: "CustomMealId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomMealOptions_OptionId",
                schema: "NutriPlan",
                table: "CustomMealOptions",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomMeals_BaseMealId",
                schema: "NutriPlan",
                table: "CustomMeals",
                column: "BaseMealId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMenuMeals_DailyMenuId",
                schema: "NutriPlan",
                table: "DailyMenuMeals",
                column: "DailyMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMenuMeals_MealId",
                schema: "NutriPlan",
                table: "DailyMenuMeals",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMenus_Date",
                schema: "NutriPlan",
                table: "DailyMenus",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMenus_MealPlanId",
                schema: "NutriPlan",
                table: "DailyMenus",
                column: "MealPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMenus_MealTypeId",
                schema: "NutriPlan",
                table: "DailyMenus",
                column: "MealTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMenus_MenuId",
                schema: "NutriPlan",
                table: "DailyMenus",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Extras_IngredientId",
                schema: "NutriPlan",
                table: "Extras",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_Extras_MealId",
                schema: "NutriPlan",
                table: "Extras",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientAllergens_AllergenId",
                schema: "NutriPlan",
                table: "IngredientAllergens",
                column: "AllergenId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientAllergens_IngredientId",
                schema: "NutriPlan",
                table: "IngredientAllergens",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientAttributes_IngredientId",
                schema: "NutriPlan",
                table: "IngredientAttributes",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientAttributes_NutrientsAttributeId",
                schema: "NutriPlan",
                table: "IngredientAttributes",
                column: "NutrientsAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientMeasurementUnits_IngredientId",
                schema: "NutriPlan",
                table: "IngredientMeasurementUnits",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientMeasurementUnits_MeasurementUnitId",
                schema: "NutriPlan",
                table: "IngredientMeasurementUnits",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_CategoryId",
                schema: "NutriPlan",
                table: "Ingredients",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MealAllergens_AllergenId",
                schema: "NutriPlan",
                table: "MealAllergens",
                column: "AllergenId");

            migrationBuilder.CreateIndex(
                name: "IX_MealAllergens_MealId",
                schema: "NutriPlan",
                table: "MealAllergens",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_MealCustomizationOptions_GroupId",
                schema: "NutriPlan",
                table: "MealCustomizationOptions",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MealCustomizationOptions_IngredientId",
                schema: "NutriPlan",
                table: "MealCustomizationOptions",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_MealCustomizationOptions_MealId",
                schema: "NutriPlan",
                table: "MealCustomizationOptions",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanMealTypes_MealPlanId",
                schema: "NutriPlan",
                table: "MealPlanMealTypes",
                column: "MealPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanMealTypes_MealTypeId",
                schema: "NutriPlan",
                table: "MealPlanMealTypes",
                column: "MealTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_RecipeId",
                schema: "NutriPlan",
                table: "Meals",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_IngredientId",
                schema: "NutriPlan",
                table: "RecipeIngredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_RecipeId",
                schema: "NutriPlan",
                table: "RecipeIngredients",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeMealTypes_MealTypeId",
                schema: "NutriPlan",
                table: "RecipeMealTypes",
                column: "MealTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeMealTypes_RecipeId",
                schema: "NutriPlan",
                table: "RecipeMealTypes",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CategoryId",
                schema: "NutriPlan",
                table: "Recipes",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomMealOptions",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "DailyMenuMeals",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "Extras",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "IngredientAllergens",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "IngredientAttributes",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "IngredientMeasurementUnits",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "MealAllergens",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "MealPlanMealTypes",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "RecipeIngredients",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "RecipeMealTypes",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "CustomMeals",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "MealCustomizationOptions",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "DailyMenus",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "NutrientsAttributes",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "MeasurementUnits",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "Allergens",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "Ingredients",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "MealCustomizationGroups",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "Meals",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "MealPlans",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "MealTypes",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "Menus",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "Recipes",
                schema: "NutriPlan");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "NutriPlan");
        }
    }
}
