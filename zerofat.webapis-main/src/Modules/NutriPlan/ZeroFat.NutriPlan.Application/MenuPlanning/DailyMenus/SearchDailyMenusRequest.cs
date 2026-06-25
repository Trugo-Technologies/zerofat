using Mapster;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenus;
public class SearchDailyMenusRequest : PaginationFilter, IQuery<PaginationResponse<DailyMenuDto>>
{
    public DateOnly? Date { get; set; }
    public DefaultIdType? MenuId { get; set; }
    public DefaultIdType? MealPlanId { get; set; }
    public DefaultIdType? MealTypeId { get; set; }
}


public class SearchDailyMenusRequestHandler(IReadRepository<DailyMenu> repository) : IQueryHandler<SearchDailyMenusRequest, PaginationResponse<DailyMenuDto>>
{
    private readonly IReadRepository<DailyMenu> _repository = repository;

    public async Task<PaginationResponse<DailyMenuDto>> Handle(SearchDailyMenusRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<DailyMenu, DailyMenuDto>();
                // .Map(destination => destination.MealTypes, src => src.DailyMenuMealTypes.Select(x => x.MealType));

        return await _repository.PaginatedListAsync(new DailyMenusBySearchRequestSpec(request), request.PageNumber, request.PageSize, config, cancellationToken);
    }
}
