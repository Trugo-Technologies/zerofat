using Mapster;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenuMeals;
public class SearchCurrentDailyMenuMealsRequest : PaginationFilter, IQuery<PaginationResponse<MenuMealDto>>
{
    public DefaultIdType? MealTypeId { get; set; }
    public DefaultIdType? MealPlanId { get; set; }
}

public class SearchCurrentDailyMenuMealsRequestHandler(IReadRepository<DailyMenuMeal> repository) : IQueryHandler<SearchCurrentDailyMenuMealsRequest, PaginationResponse<MenuMealDto>>
{
    private readonly IReadRepository<DailyMenuMeal> _repository = repository;

    public async Task<PaginationResponse<MenuMealDto>> Handle(SearchCurrentDailyMenuMealsRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new();
        config.NewConfig<DailyMenuMeal, MenuMealDto>()
              .Map(destination => destination.Date, src => src.DailyMenu.Date);

        var result = await _repository.PaginatedListAsync(new CurrentDailyMenuMealsBySearchRequestSpec(request), request.PageNumber, request.PageSize, config, cancellationToken);
        foreach (var item in result.Data)
        {
            item.NameAr = item.Meal.NameAr;
            item.NameEn = item.Meal.NameEn;
            item.ImageUrl = item.Meal.ImageUrl;
        }

        return result;
    }
}
