using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationGroups;
public class SearchMealCustomizationGroupsRequest : PaginationFilter, IQuery<PaginationResponse<MealCustomizationGroupDto>>
{
    public DefaultIdType? MealId { get; set; }
}


public class SearchMealCustomizationGroupsRequestHandler(IReadRepository<MealCustomizationGroup> repository) : IRequestHandler<SearchMealCustomizationGroupsRequest, PaginationResponse<MealCustomizationGroupDto>>
{
    private readonly IReadRepository<MealCustomizationGroup> _repository = repository;

    public async Task<PaginationResponse<MealCustomizationGroupDto>> Handle(SearchMealCustomizationGroupsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new MealCustomizationGroupsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
