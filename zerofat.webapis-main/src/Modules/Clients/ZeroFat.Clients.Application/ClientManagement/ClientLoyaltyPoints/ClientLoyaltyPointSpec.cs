using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientLoyaltyPoints;
public class ClientLoyaltyPointsBySearchRequestSpec : EntitiesByPaginationFilterSpec<ClientLoyaltyPoint, ClientLoyaltyPointDto>
{
    public ClientLoyaltyPointsBySearchRequestSpec(SearchClientLoyaltyPointsRequest request)
        : base(request)
    {
        Query.Where(x => x.ClientId == request.ClientId, request.ClientId.HasValue)
             .OrderByDescending(x => x.DateEarned);
    }
}

public class ClientLoyaltyPointByIdSpec<T> : Specification<ClientLoyaltyPoint, T>
{
    public ClientLoyaltyPointByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

public class CurrentClientLoyaltyPointByIdSpec<T> : Specification<ClientLoyaltyPoint, T>
{
    public CurrentClientLoyaltyPointByIdSpec(DefaultIdType clientId)
    {
        Query.Where(p => p.ClientId == clientId);
    }
}
