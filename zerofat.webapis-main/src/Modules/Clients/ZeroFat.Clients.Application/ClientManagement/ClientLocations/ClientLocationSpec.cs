using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.ClientManagement.ClientLocations;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement;
public class ClientLocationsBySearchRequestSpec : EntitiesByPaginationFilterSpec<ClientLocation, ClientLocationSimplifyDto>
{
    public ClientLocationsBySearchRequestSpec(SearchClientLocationsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.ClientId == request.ClientId, request.ClientId.HasValue)
             .Where(x => x.Type == request.Type, request.Type.HasValue);
    }
}

public class ClientLocationByIdSpec<T> : Specification<ClientLocation, T>
{
    public ClientLocationByIdSpec(Guid id) => Query.Where(p => p.Id == id);
}

