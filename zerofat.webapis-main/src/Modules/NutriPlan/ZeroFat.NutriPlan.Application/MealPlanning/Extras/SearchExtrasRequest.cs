using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Extras;
public class SearchExtrasRequest : PaginationFilter, IQuery<PaginationResponse<ExtraDto>>
{
    public DefaultIdType? MealId { get; set; }
}


public class SearchExtrasRequestHandler(IReadRepository<Extra> repository) : IRequestHandler<SearchExtrasRequest, PaginationResponse<ExtraDto>>
{
    private readonly IReadRepository<Extra> _repository = repository;

    public async Task<PaginationResponse<ExtraDto>> Handle(SearchExtrasRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new ExtrasBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
