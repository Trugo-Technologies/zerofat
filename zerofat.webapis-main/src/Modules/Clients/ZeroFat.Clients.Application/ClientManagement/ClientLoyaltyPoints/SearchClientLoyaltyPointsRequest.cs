using MediatR;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientLoyaltyPoints;
public class SearchClientLoyaltyPointsRequest : PaginationFilter, IQuery<PaginationResponse<ClientLoyaltyPointDto>>
{
    public DefaultIdType? ClientId { get; set; }

}


public class SearchClientLoyaltyPointsRequestHandler(IReadRepository<ClientLoyaltyPoint> repository, ICurrentUser currentUser ) : IRequestHandler<SearchClientLoyaltyPointsRequest, PaginationResponse<ClientLoyaltyPointDto>>
{
    private readonly IReadRepository<ClientLoyaltyPoint> _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<PaginationResponse<ClientLoyaltyPointDto>> Handle(SearchClientLoyaltyPointsRequest request, CancellationToken cancellationToken)
    {
        bool isClient = _currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (isClient)
        {
            request.ClientId = _currentUser.GetUserId();
        }

        return await _repository.PaginatedListAsync(new ClientLoyaltyPointsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);
    }

}
