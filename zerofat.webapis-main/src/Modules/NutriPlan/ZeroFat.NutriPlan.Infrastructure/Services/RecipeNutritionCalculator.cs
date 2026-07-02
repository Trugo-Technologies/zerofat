using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.Infrastructure.BackgroundProcessing.Contracts;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Domain.Settings;
using ZeroFat.NutriPlan.Infrastructure.Persistence.Context;

namespace ZeroFat.NutriPlan.Infrastructure.Services;

/// <summary>
/// Handles recipe nutrition calculations and updates related meal nutritional information
/// </summary>
public class RecipeNutritionCalculator : IRecipeNutritionCalculator
{
    private readonly NutriPlanContext _dbContext;
    private readonly ILogger<RecipeNutritionCalculator> _logger;

    /// <summary>
    /// Initializes a new instance of the RecipeNutritionCalculator
    /// </summary>
    /// <param name="dbContext">Database context for NutriPlan application</param>
    public RecipeNutritionCalculator(
        NutriPlanContext dbContext, 
        ILogger<RecipeNutritionCalculator> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Calculates and updates nutritional values for all recipes and their associated meals
    /// based on ingredient composition. Uses USDA standard calculations.
    /// </summary>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task CalculateAndUpdateNutritionalValuesAsync()
    {
        try
        {
            // Load all recipes with their ingredients and related meals
            var recipes = await _dbContext.Recipes
                .Include(x => x.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient).ThenInclude(x => x.IngredientAllergens)
                .Include(x => x.Meals).ThenInclude(x => x.Allergens)
                .ToListAsync();

            foreach (var recipe in recipes)
            {
                CalculateRecipeNutrition(recipe);
                UpdateAssociatedMealsNutrition(recipe);
            }

            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Failed to calculate recipe nutritional values");
            // Log the exception here if you have logging infrastructure
            // throw new ApplicationException("Failed to calculate recipe nutritional values", ex);
        }
    }

    /// <summary>
    /// Calculates the nutritional values for a single recipe based on its ingredients
    /// </summary>
    /// <param name="recipe">The recipe to calculate nutrition for</param>
    private void CalculateRecipeNutrition(Recipe recipe)
    {
        // Reset all nutritional values before calculation
        recipe.WeightInGrams = 0;
        recipe.Protein = 0;
        recipe.Water = 0;
        recipe.Calories = 0;
        recipe.Carbs = 0;
        recipe.Fat = 0;

        // Sum nutritional values from all ingredients
        foreach (var recipeIngredient in recipe.RecipeIngredients)
        {
            var ingredient = recipeIngredient.Ingredient;
            var weightRatio = recipeIngredient.WeightInGrams / 100.0;

            recipe.WeightInGrams += recipeIngredient.WeightInGrams;
            recipe.Protein += ingredient.ProteinPer100Unit * weightRatio;
            recipe.Water += ingredient.WaterPer100g * weightRatio;
            recipe.Calories += ingredient.CaloriesPer100Unit * weightRatio;
            recipe.Carbs += ingredient.CarbsPer100Unit * weightRatio;
            recipe.Fat += ingredient.FatPer100Unit * weightRatio;
        }
    }

    /// <summary>
    /// Updates all meals associated with this recipe to match the recipe's nutritional values
    /// </summary>
    /// <param name="recipe">The recipe whose meals should be updated</param>
    private void UpdateAssociatedMealsNutrition(Recipe recipe)
    {
        var ingredientAllergenIds = recipe.RecipeIngredients
               .SelectMany(ri => ri.Ingredient.IngredientAllergens)
               .Select(ia => ia.AllergenId)
               .Distinct()
               .ToHashSet();

        foreach (var meal in recipe.Meals)
        {
            // 1️⃣ Remove allergens in the meal that are no longer in the recipe ingredients
            var allergensToRemove = meal.Allergens
                .Where(ma => !ingredientAllergenIds.Contains(ma.AllergenId))
                .ToList();

            foreach (var allergen in allergensToRemove)
            {
                _dbContext.Remove(allergen); // ensures EF Core marks for deletion
            }

            // 2️⃣ Add allergens that are in the ingredients but missing from the meal
            var existingAllergenIds = meal.Allergens
                .Select(ma => ma.AllergenId)
                .ToHashSet();

            var allergensToAdd = ingredientAllergenIds
                .Where(aid => !existingAllergenIds.Contains(aid))
                .Select(aid => new MealAllergen
                {
                    AllergenId = aid,
                    MealId = meal.Id
                });

            _dbContext.AddRange(allergensToAdd);

            // Update basic nutritional values
            meal.Calories = recipe.Calories;
            meal.Protein = recipe.Protein;
            meal.Water = recipe.Water;
            meal.WeightInGrams = recipe.WeightInGrams;
            meal.Carbs = recipe.Carbs;
            meal.Fat = recipe.Fat;

            meal.FullRecipeTextEn = recipe.FullRecipeTextEn;
            meal.FullRecipeTextAr = recipe.FullRecipeTextAr;
            meal.PreparationMethodEn = recipe.PreparationMethodEn;
            meal.PreparationMethodAr = recipe.PreparationMethodAr;

            // Update dietary properties
            meal.Cuisine = recipe.Cuisine;
            meal.IsCold = recipe.IsCold;
            meal.IsWarm = recipe.IsWarm;
            meal.IsDairyFree = recipe.IsDairyFree;
            meal.DietaryCategories = recipe.DietaryCategories;
            meal.IsGlutenFree = recipe.IsGlutenFree;
            meal.IsLowGI = recipe.IsLowGI;
        }
    }
}


public class NutritionCalculationJob : IRecurringBackgroundJobScheduler
{
    private readonly IRecipeNutritionCalculator _calculator;

    public NutritionCalculationJob(IRecipeNutritionCalculator calculator)
    {
        _calculator = calculator;
    }

    public Task ScheduleAsync(IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate(
            "nutrition-calculation-daily",
            () => _calculator.CalculateAndUpdateNutritionalValuesAsync(),
            Cron.Daily(3)); // 3 AM UTC
        return Task.CompletedTask;
    }
}

