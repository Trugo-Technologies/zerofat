using Mapster;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Ingredients;
public class SearchIngredientsRequest : PaginationFilter, IQuery<PaginationResponse<IngredientDto>>
{
}


public class SearchIngredientsRequestHandler(IReadRepository<Ingredient> repository) : IQueryHandler<SearchIngredientsRequest, PaginationResponse<IngredientDto>>
{
    private readonly IReadRepository<Ingredient> _repository = repository;

    public async Task<PaginationResponse<IngredientDto>> Handle(SearchIngredientsRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<Ingredient, IngredientDto>()
                .Map(destination => destination.Allergens, src => src.IngredientAllergens.Select(x => x.Allergen));

        return await _repository.PaginatedListAsync(new IngredientsBySearchRequestSpec(request), request.PageNumber, request.PageSize, config, cancellationToken);
    }
}
