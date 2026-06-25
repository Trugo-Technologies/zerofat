using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenuMeals;
public class SearchDailyMenuMealsRequest : PaginationFilter, IQuery<PaginationResponse<DailyMenuMealDto>>
{
    public DateOnly? Date { get; set; }
    public DefaultIdType? MealTypeId { get; set; }
    public DefaultIdType? MealPlanId { get; set; }
    public DefaultIdType? MenuId { get; set; }
}


public class SearchDailyMenuMealsRequestHandler(IReadRepository<DailyMenuMeal> repository) : IQueryHandler<SearchDailyMenuMealsRequest, PaginationResponse<DailyMenuMealDto>>
{
    private readonly IReadRepository<DailyMenuMeal> _repository = repository;

    public async Task<PaginationResponse<DailyMenuMealDto>> Handle(SearchDailyMenuMealsRequest request, CancellationToken cancellationToken)
    {
        return await _repository.PaginatedListAsync(new DailyMenuMealsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);
    }
}
