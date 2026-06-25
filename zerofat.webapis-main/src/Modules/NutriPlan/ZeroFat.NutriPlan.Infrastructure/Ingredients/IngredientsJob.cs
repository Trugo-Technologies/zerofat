using Hangfire.Console.Extensions;
using Hangfire.Console.Progress;
using Hangfire.Logging;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Infrastructure.Persistence.Context;

namespace ZeroFat.NutriPlan.Infrastructure.Ingredients;

public class IngredientsJob : IIngredientsJob
{
    // private readonly AcctivateDbContext _context;
    private readonly NutriPlanContext _dbContext;
    private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
    private readonly IProgressBar _progress;
    private readonly IProgressBarFactory _progressBar;

    public IngredientsJob(
        NutriPlanContext dbContext,
        IProgressBarFactory progressBar)
    {
        _dbContext = dbContext;
        _progressBar = progressBar;
        _progress = _progressBar.Create();
    }

    public async Task GetUSDAIngredientsAsync()
    {
        try
        {
            int pageNumber = 1;
            var url = "https://api.nal.usda.gov/fdc/v1/foods/search";

            //while (true)
            //{
            //    var result = await url.SetQueryParam("api_key", "qoYumgF0OtPw0VPVOGbmEnwZNukaJDaZna1lf1fB")
            //                          .SetQueryParam("pageNumber", pageNumber)
            //                          .SetQueryParam("pageSize", 10)
            //                          .SetQueryParam("dataType", "Foundation")
            //                          .GetJsonAsync<USDAFoodSearchResponse>();

            //    foreach(var ingredient in result.Foods)
            //    {
            //        var category = await _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryType == CategoryType.Ingredient && x.NameEn == ingredient.FoodCategory);
            //        if(category == null)
            //        {
            //            category = new Category()
            //            {
            //                CategoryType = CategoryType.Ingredient,
            //                NameEn = ingredient.FoodCategory,
            //                NameAr = ingredient.FoodCategory,
            //            };

            //            await _dbContext.Categories.AddAsync(category);
            //        }

            //        var ingredientInstance = new Ingredient()
            //        {
            //            IngredientSource = IngredientSource.USDA,
            //            NameEn = ingredient.ScientificName,
            //            NameAr = ingredient.ScientificName,
            //            DescriptionAr = ingredient.Description,
            //            DescriptionEn = ingredient.Description,

            //            CategoryId = category.Id,
            //            DietaryPreference = DietaryPreference.None,
            //            BasicUnit = BasicUnitType.Solid,
            //            Type = IngredientType.Other,
            //            Status = IngredientStatus.Available,
            //            IsDairyFree = false,
            //            IsGlutenFree = false,
            //            IsLowGI = false,
            //            IsOrganic = false,
            //            IsSeasonal = false,
            //            Density = 1,

            //            CostPer100Unit = new Random().Next(1000),

            //            CaloriesPer100Unit = request.CaloriesPer100Unit,
            //            CarbsPer100Unit = request.CarbsPer100Unit,
            //            FatPer100Unit = request.FatPer100Unit,
            //            FiberPer100Unit = request.FiberPer100Unit,
            //            SugarPer100Unit = request.SugarPer100Unit,
            //            VitaminContent = request.VitaminContent,
            //            CaloriesUnit = BasicUnit.g,
            //            MineralContent = request.MineralContent,
            //        };
            //        foreach(var foodNutrient in ingredient.FoodNutrients)
            //        {
            //            if(foodNutrient.NutrientName == "Protein")
            //            {
            //                ingredientInstance.ProteinPer100Unit = foodNutrient.Value;
            //            }
            //            else if (foodNutrient.NutrientName == "Water")
            //            {
            //                ingredientInstance.WaterPer100g = foodNutrient.Value;
            //            }
            //            else if (foodNutrient.NutrientName == "Energy (Atwater General Factors)")
            //            {
            //                ingredientInstance.CaloriesPer100Unit = foodNutrient.Value;
            //            }
            //            else if (foodNutrient.NutrientName == "Energy (Atwater General Factors)")
            //            {
            //                ingredientInstance.CaloriesPer100Unit = foodNutrient.Value;
            //            }
            //        }

            //        await _dbContext.Ingredients.AddAsync(ingredientInstance);
            //    }

            //    pageNumber++;
            //}



        }
        catch (Exception e)
        {
            throw e;
        }
    }
}

public class UsdaFoodSearchResponse
{
    public int TotalHits { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public List<UsdaIngredient>? Foods { get; set; }
}

public class UsdaFoodNutrient
{
    public int NutrientId { get; set; }
    public string? NutrientName { get; set; }
    public string? NutrientNumber { get; set; }
    public string? UnitName { get; set; }
    public double Value { get; set; }
}

public class UsdaIngredient
{
    public int FdcId { get; set; }
    public string? Description { get; set; }
    public string? ScientificName { get; set; }
    public string? FoodCategory { get; set; }
    public List<UsdaFoodNutrient>? FoodNutrients { get; set; }
}
