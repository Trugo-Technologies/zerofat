using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.NutrientsAttributes;
public class SearchNutrientsAttributesRequest : PaginationFilter, IQuery<PaginationResponse<NutrientsAttributeDto>>
{
}


public class SearchNutrientsAttributesRequestHandler(IReadRepository<NutrientsAttribute> repository) : IQueryHandler<SearchNutrientsAttributesRequest, PaginationResponse<NutrientsAttributeDto>>
{
    private readonly IReadRepository<NutrientsAttribute> _repository = repository;

    public async Task<PaginationResponse<NutrientsAttributeDto>> Handle(SearchNutrientsAttributesRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new NutrientsAttributesBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
