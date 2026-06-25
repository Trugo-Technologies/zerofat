using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Recipes;
public class RecipesBySearchRequestSpec : EntitiesByPaginationFilterSpec<Recipe, RecipeDto>
{
    public RecipesBySearchRequestSpec(SearchRecipesRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class RecipeByIdSpec<T> : Specification<Recipe, T>
{
    public RecipeByIdSpec(Guid id) => Query.Where(p => p.Id == id);
}

