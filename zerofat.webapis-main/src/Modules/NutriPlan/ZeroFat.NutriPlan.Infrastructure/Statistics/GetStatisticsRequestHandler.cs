using MediatR;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Application.Statistics;
using ZeroFat.NutriPlan.Infrastructure.Persistence.Context;

namespace ZeroFat.NutriPlan.Infrastructure.Statistics;

public class GetStatisticsRequestHandler(NutriPlanContext db) : IRequestHandler<GetStatisticsRequest, Result<StatisticsDto>>
{
    private readonly NutriPlanContext _db = db;

    public async Task<Result<StatisticsDto>> Handle(GetStatisticsRequest request, CancellationToken cancellationToken)
    {

        return await Result<StatisticsDto>.SuccessAsync(new StatisticsDto
        {
            MealPlans = await _db.MealPlans.CountAsync(x => x.DeletedOn == null && x.IsActive, cancellationToken),
            Ingredients = await _db.Ingredients.CountAsync(x => x.DeletedOn == null, cancellationToken),
            Menus = await _db.Menus.CountAsync(x => x.DeletedOn == null, cancellationToken),
            PublishedMenus = await _db.Menus.CountAsync(x => x.DeletedOn == null && x.IsPublished, cancellationToken),
            Extras = await _db.Extras.CountAsync(x => x.DeletedOn == null, cancellationToken),
            Meals = await _db.Meals.CountAsync(x => x.DeletedOn == null && x.IsActive, cancellationToken),
            Recipes = await _db.Recipes.CountAsync(x => x.DeletedOn == null, cancellationToken),
            ColdRecipes = await _db.Recipes.CountAsync(x => x.DeletedOn == null && x.IsCold, cancellationToken),
            WarmRecipes = await _db.Recipes.CountAsync(x => x.DeletedOn == null && x.IsWarm, cancellationToken),
            IngredientsByStatus = await GetIngredientsByStatus(),
            IngredientsByType = await GetIngredientsByType(),
            MealByCuisineType = await GetMealByCuisineType(),
            RecipeByDifficulty = await GetRecipeByDifficulty(),
        });
    }


    private async Task<Dictionary<string, int>> GetIngredientsByStatus()
    {
        return await _db.Ingredients.Where(x => x.DeletedOn == null)
                               .GroupBy(x => x.Status)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }

    private async Task<Dictionary<string, int>> GetIngredientsByType()
    {
        return await _db.Ingredients.Where(x => x.DeletedOn == null)
                               .GroupBy(x => x.Type)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }

    private async Task<Dictionary<string, int>> GetMealByCuisineType()
    {
        return await _db.Meals.Where(x => x.Cuisine != null && x.DeletedOn == null && x.IsActive)
                               .GroupBy(x => x.Cuisine!.Value)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }

    private async Task<Dictionary<string, int>> GetRecipeByDifficulty()
    {
        return await _db.Recipes.Where(x => x.DeletedOn == null)
                               .GroupBy(x => x.Difficulty)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }
}
