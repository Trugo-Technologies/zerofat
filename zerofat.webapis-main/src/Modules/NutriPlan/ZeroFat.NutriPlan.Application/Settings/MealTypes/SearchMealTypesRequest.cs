using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealTypes;
public class SearchMealTypesRequest : PaginationFilter, IQuery<PaginationResponse<MealTypeDto>>
{

}


public class SearchMealTypesRequestHandler(IReadRepository<MealType> repository) : IQueryHandler<SearchMealTypesRequest, PaginationResponse<MealTypeDto>>
{
    private readonly IReadRepository<MealType> _repository = repository;

    public async Task<PaginationResponse<MealTypeDto>> Handle(SearchMealTypesRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new MealTypesBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
