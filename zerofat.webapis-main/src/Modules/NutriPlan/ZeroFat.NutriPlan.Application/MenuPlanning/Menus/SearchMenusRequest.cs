using Mapster;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.Menus;
public class SearchMenusRequest : PaginationFilter, IQuery<PaginationResponse<MenuDto>>
{
}


public class SearchMenusRequestHandler(IReadRepository<Menu> repository) : IQueryHandler<SearchMenusRequest, PaginationResponse<MenuDto>>
{
    private readonly IReadRepository<Menu> _repository = repository;

    public async Task<PaginationResponse<MenuDto>> Handle(SearchMenusRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<Menu, MenuDto>();
                // .Map(destination => destination.MealTypes, src => src.MenuMealTypes.Select(x => x.MealType));

        return await _repository.PaginatedListAsync(new MenusBySearchRequestSpec(request), request.PageNumber, request.PageSize, config, cancellationToken);
    }
}
