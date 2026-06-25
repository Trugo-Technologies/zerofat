using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MenuPlanning;
using ZeroFat.NutriPlan.Infrastructure.Persistence.Context;
using ZeroFat.NutriPlan.Infrastructure.Services;

namespace ZeroFat.NutriPlan.Infrastructure.Seedings.NutriPlan;
public class RecipesSeeder : INutriPlanSeeder
{
    private readonly NutriPlanContext _db;
    private readonly ILogger<RecipesSeeder> _logger;

    public RecipesSeeder(ILogger<RecipesSeeder> logger, NutriPlanContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.Recipes.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed Recipes.");
                string recipesData = await File.ReadAllTextAsync(path + "/Seedings/NutriPlan/recipes.json", cancellationToken);
                var recipes = JsonSerializer.Deserialize<List<Recipe>>(recipesData);
                var categories = await _db.Categories.ToListAsync(cancellationToken);
                var measurementUnits = await _db.MeasurementUnits.Where(x=>x.IsDefault).ToListAsync(cancellationToken);
                var allergens = await _db.Allergens.ToListAsync(cancellationToken);
                var mealTypes = await _db.MealTypes.ToListAsync(cancellationToken);
                var mealPlans = await _db.MealPlans.Include(x => x.MealPlanMealTypes).ThenInclude(x => x.MealType).ToListAsync(cancellationToken);

                if (recipes != null)
                {
                    foreach (var recipe in recipes)
                    {
                        recipe.CategoryId = categories.Where(x=>x.CategoryType == CategoryType.Recipe).OrderBy(x => Guid.NewGuid()).FirstOrDefault()?.Id;
                        var selectedMealTypes = mealTypes.OrderBy(x => Guid.NewGuid()).Take(4).ToList().ConvertAll(x => new RecipeMealType() { MealTypeId = x.Id });
                        recipe.RecipeMealTypes.AddRange(selectedMealTypes);
                        foreach (var recipeIngredien in recipe.RecipeIngredients)
                        {
                            recipeIngredien.WeightInGrams = recipeIngredien.Amount * recipeIngredien.Ingredient.Density;
                            recipeIngredien.Ingredient.CostPer100Unit *= 100;
                            if(recipeIngredien.Ingredient.BasicUnit == BasicUnitType.Solid)
                            {
                                recipeIngredien.BasicUnit = BasicUnit.g;
                                recipeIngredien.MeasurementUnitCode = "G";
                            }
                            else
                            {
                                recipeIngredien.BasicUnit = BasicUnit.ml;
                                recipeIngredien.MeasurementUnitCode = "ML";
                            }

                            var list = allergens.OrderBy(x => Guid.NewGuid()).Take(2).ToList().ConvertAll(x => new IngredientAllergen() { AllergenId = x.Id });
                            recipeIngredien.Ingredient.IngredientAllergens.AddRange(list);
                            recipeIngredien.Ingredient.CategoryId = categories.Where(x => x.CategoryType == CategoryType.Ingredient).OrderBy(x => Guid.NewGuid()).FirstOrDefault()?.Id;


                            recipe.WeightInGrams += recipeIngredien.WeightInGrams;
                            recipe.Protein += recipeIngredien.Ingredient.ProteinPer100Unit * recipeIngredien.WeightInGrams / 100;
                            recipe.Water += recipeIngredien.Ingredient.WaterPer100g * recipeIngredien.WeightInGrams / 100;
                            recipe.Calories += recipeIngredien.Ingredient.CaloriesPer100Unit * recipeIngredien.WeightInGrams / 100;
                            recipe.Carbs += recipeIngredien.Ingredient.CarbsPer100Unit * recipeIngredien.WeightInGrams / 100;
                            recipe.Fat += recipeIngredien.Ingredient.FatPer100Unit * recipeIngredien.WeightInGrams / 100;
                        }
                    }

                    await _db.Recipes.AddRangeAsync(recipes, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }


                var recipesInDb = await _db.Recipes.Include(x => x.RecipeMealTypes).ToListAsync(cancellationToken);

                foreach(var recipe in recipesInDb)
                {
                    var meal = new Meal()
                    {
                        RecipeId = recipe.Id,
                        Water = recipe.Water,
                        WeightInGrams = recipe.WeightInGrams,
                        SuitableForFreezing = false,
                        Protein = recipe.Protein,
                        Fat =  recipe.Fat,
                        Calories = recipe.Calories,
                        Cuisine = recipe.Cuisine,
                        NameAr = recipe.NameAr,
                        NameEn = recipe.NameEn,
                        IsWarm = recipe.IsWarm,
                        IsActive = true,
                        IsAddOn = true,
                        IsCold = recipe.IsCold,
                        IsDairyFree = recipe.IsDairyFree,
                        IsGlutenFree = recipe.IsGlutenFree,
                        Carbs = recipe.Carbs,
                        Allergens = allergens.OrderBy(x => Guid.NewGuid()).Take(3).ToList().ConvertAll(x=> new MealAllergen() { AllergenId = x.Id}),
                        DietaryCategories = recipe.DietaryCategories,
                        IsLowGI = recipe.IsLowGI,
                        ImageUrl = recipe.ImageUrl,
                        PriceForCustomer = new Random().Next(100,200),
                    };

                    await _db.Meals.AddAsync(meal, cancellationToken);
                }

                await _db.SaveChangesAsync(cancellationToken);
                var mealsdb = await _db.Meals.Include(x => x.Recipe).ThenInclude(x => x.RecipeMealTypes).ToListAsync(cancellationToken);
                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1,0,0,0, DateTimeKind.Utc);
                for (int i = 0; i < 14; i++)
                {
                    var menu = new Menu()
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(7),
                        IsPublished = true,
                        NameEn = "Menu Test",
                        NameAr = "Menu Test"
                    };

                    for (DateTime date = menu.StartDate; date <= menu.EndDate; date = date.AddDays(1))
                    {
                        foreach (var mealPlan in mealPlans)
                        {
                            foreach (var mealType in mealPlan.MealPlanMealTypes)
                            {
                                var dailyMenu = new DailyMenu()
                                {
                                    Date = date,
                                    MealTypeId = mealType.MealTypeId,
                                    MealPlanId = mealPlan.Id,
                                    Price = mealType.Price,
                                    AverageCalories = mealType.AverageCalories,
                                };

                                bool isDefault = true;

                                foreach(var meal in mealsdb.Where(x => x.Recipe.RecipeMealTypes.Any(x=>x.MealTypeId == mealType.MealTypeId)).Take(2))
                                {
                                    dailyMenu.Meals.Add(new DailyMenuMeal()
                                    {
                                        IsDefault = isDefault,
                                        MealId = meal.Id,
                                    });

                                    isDefault = false;
                                }

                                menu.DailyMenus.Add(dailyMenu);
                            }
                        }
                    }

                    await _db.Menus.AddAsync(menu, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                    startDate = startDate.AddDays(7);
                }
                _logger.LogInformation("Seed NutriPlan Recipes Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding NutriPlan Recipes.");
            }
        }
    }

    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder result = new StringBuilder(length);
        Random random = new Random();

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        return result.ToString();
    }
}
