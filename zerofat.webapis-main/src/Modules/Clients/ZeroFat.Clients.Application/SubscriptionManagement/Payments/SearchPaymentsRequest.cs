using MediatR;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Payments;

public class SearchPaymentsRequest : PaginationFilter, IQuery<PaginationResponse<PaymentDto>>
{
    public DefaultIdType? ClientId { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
}


public class SearchPaymentsRequestHandler(IReadRepository<Payment> repository, ICurrentUser currentUser) : IRequestHandler<SearchPaymentsRequest, PaginationResponse<PaymentDto>>
{
    private readonly IReadRepository<Payment> _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<PaginationResponse<PaymentDto>> Handle(SearchPaymentsRequest request, CancellationToken cancellationToken)
    {
        bool isClient = _currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (isClient)
            request.ClientId = _currentUser.GetUserId();
        var result = await _repository.PaginatedListAsync(new PaymentsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);
        return result;
    }

}
