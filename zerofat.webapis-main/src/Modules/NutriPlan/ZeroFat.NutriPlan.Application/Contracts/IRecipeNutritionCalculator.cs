using Hangfire;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.NutriPlan.Application.Contracts;

/// <summary>
/// Calculates and updates nutritional values for recipes and their associated meals.
/// </summary>
public interface IRecipeNutritionCalculator : ITransientService
{
    /// <summary>
    /// Processes all recipes, calculates their nutritional values, and updates related meals.
    /// Uses USDA-standard calculations.
    /// </summary>
    /// <remarks>
    /// - Runs as a background job via Hangfire ("default" queue)
    /// - No automatic retries to avoid duplicate updates
    /// </remarks>
    [Queue("default")]  // Explicit queue name for job processing
    [AutomaticRetry(Attempts = 0)]  // Disable retries for data consistency
    Task CalculateAndUpdateNutritionalValuesAsync();
}
