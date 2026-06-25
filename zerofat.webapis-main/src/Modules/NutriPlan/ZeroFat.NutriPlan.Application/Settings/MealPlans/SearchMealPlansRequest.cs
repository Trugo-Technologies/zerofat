using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealPlans;
public class SearchMealPlansRequest : PaginationFilter, IQuery<PaginationResponse<MealPlanDto>>
{
    public bool? IsActive { get; set; }
}


public class SearchMealPlansRequestHandler(IReadRepository<MealPlan> repository) : IQueryHandler<SearchMealPlansRequest, PaginationResponse<MealPlanDto>>
{
    private readonly IReadRepository<MealPlan> _repository = repository;

    public async Task<PaginationResponse<MealPlanDto>> Handle(SearchMealPlansRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new MealPlansBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
