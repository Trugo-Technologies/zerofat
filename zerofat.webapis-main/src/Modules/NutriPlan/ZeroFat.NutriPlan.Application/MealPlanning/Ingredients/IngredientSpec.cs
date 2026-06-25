using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Ingredients;
public class IngredientsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Ingredient, IngredientDto>
{
    public IngredientsBySearchRequestSpec(SearchIngredientsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class IngredientByIdSpec<T> : Specification<Ingredient, T>
{
    public IngredientByIdSpec(Guid id) => Query.Where(p => p.Id == id);
}

