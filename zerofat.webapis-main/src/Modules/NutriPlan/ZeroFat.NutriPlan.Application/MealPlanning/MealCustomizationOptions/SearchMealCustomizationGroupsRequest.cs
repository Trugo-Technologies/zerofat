using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationOptions;
public class SearchMealCustomizationOptionsRequest : PaginationFilter, IQuery<PaginationResponse<MealCustomizationOptionDto>>
{
    public DefaultIdType? MealId { get; set; }
}


public class SearchMealCustomizationOptionsRequestHandler(IReadRepository<MealCustomizationOption> repository) : IRequestHandler<SearchMealCustomizationOptionsRequest, PaginationResponse<MealCustomizationOptionDto>>
{
    private readonly IReadRepository<MealCustomizationOption> _repository = repository;

    public async Task<PaginationResponse<MealCustomizationOptionDto>> Handle(SearchMealCustomizationOptionsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new MealCustomizationOptionsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
