using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.ClientManagement;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement;

public class SearchClientSubscriptionsRequest : PaginationFilter, IQuery<PaginationResponse<ClientSubscriptionDto>>
{
    public Guid? ClientId { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public SubscriptionStatus? SubscriptionStatus { get; set; }
}


public class SearchClientSubscriptionsRequestHandler(IReadRepository<ClientSubscription> repository, IReadRepository<ClientLocation> mealPlanRepo) : IRequestHandler<SearchClientSubscriptionsRequest, PaginationResponse<ClientSubscriptionDto>>
{
    private readonly IReadRepository<ClientSubscription> _repository = repository;
    private readonly IReadRepository<ClientLocation> _mealPlanRepo = mealPlanRepo;

    public async Task<PaginationResponse<ClientSubscriptionDto>> Handle(SearchClientSubscriptionsRequest request, CancellationToken cancellationToken)
    {
        var result = await _repository.PaginatedListAsync(new ClientSubscriptionsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);
        foreach(var subscription in result.Data)
        {
            if (subscription.ClientLocationId.HasValue)
            {
                subscription.ClientLocation = await _mealPlanRepo.FirstOrDefaultAsync(new ClientLocationByIdSpec<ClientLocationSimplifyDto>(subscription.ClientLocationId!.Value), cancellationToken);
            }
        }

        return result;
    }
    
}
