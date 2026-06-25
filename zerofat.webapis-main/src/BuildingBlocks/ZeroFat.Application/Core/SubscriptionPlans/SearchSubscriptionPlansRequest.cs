using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Core.SubscriptionPlans;
public class SearchSubscriptionPlansRequest : PaginationFilter, IQuery<PaginationResponse<SubscriptionPlanSimplifyDto>>
{
    public bool? PercentageDiscount { get; set; }
    public ApplicationModule? SubscriptionModule { get; set; }
    public SubscriptionType? SubscriptionType { get; set; }
    public bool? IsActive { get; set; }
}


public class SearchSubscriptionPlansRequestHandler(IReadRepository<SubscriptionPlan> repository) : IRequestHandler<SearchSubscriptionPlansRequest, PaginationResponse<SubscriptionPlanSimplifyDto>>
{
    private readonly IReadRepository<SubscriptionPlan> _repository = repository;

    public async Task<PaginationResponse<SubscriptionPlanSimplifyDto>> Handle(SearchSubscriptionPlansRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new SubscriptionPlansBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
