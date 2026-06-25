using Mapster;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Meals;
public class SearchMealsRequest : PaginationFilter, IQuery<PaginationResponse<MealDto>>
{
    public bool? IsAddOn { get; set; }
}


public class SearchMealsRequestHandler(IReadRepository<Meal> repository) : IQueryHandler<SearchMealsRequest, PaginationResponse<MealDto>>
{
    private readonly IReadRepository<Meal> _repository = repository;

    public async Task<PaginationResponse<MealDto>> Handle(SearchMealsRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new();
        config.NewConfig<Meal, MealDto>()
                .Map(destination => destination.Allergens, src => src.Allergens.Select(x => x.Allergen));

        return await _repository.PaginatedListAsync(new MealsBySearchRequestSpec(request), request.PageNumber, request.PageSize, config, cancellationToken);
    }
}
