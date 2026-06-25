using Mapster;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Recipes;
public class SearchRecipesRequest : PaginationFilter, IQuery<PaginationResponse<RecipeDto>>
{
}


public class SearchRecipesRequestHandler(IReadRepository<Recipe> repository) : IQueryHandler<SearchRecipesRequest, PaginationResponse<RecipeDto>>
{
    private readonly IReadRepository<Recipe> _repository = repository;

    public async Task<PaginationResponse<RecipeDto>> Handle(SearchRecipesRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<Recipe, RecipeDto>()
                .Map(destination => destination.MealTypes, src => src.RecipeMealTypes.Select(x => x.MealType));

        return await _repository.PaginatedListAsync(new RecipesBySearchRequestSpec(request), request.PageNumber, request.PageSize, config, cancellationToken);
    }
}
