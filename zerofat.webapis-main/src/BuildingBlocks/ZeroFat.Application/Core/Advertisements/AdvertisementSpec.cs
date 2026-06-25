using Ardalis.Specification;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.Advertisements;
public class AdvertisementsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Advertisement, AdvertisementDto>
{
    public AdvertisementsBySearchRequestSpec(SearchAdvertisementsRequest request)
        : base(request)
    {
        Query.OrderBy(c => c.Index, !request.HasOrderBy())
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class AdvertisementByIdSpec<T> : Specification<Advertisement, T>
{
    public AdvertisementByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

public class LastAdvertisementByIndexSpec : Specification<Advertisement>
{
    public LastAdvertisementByIndexSpec() => Query.OrderByDescending(p => p.Index);
}

