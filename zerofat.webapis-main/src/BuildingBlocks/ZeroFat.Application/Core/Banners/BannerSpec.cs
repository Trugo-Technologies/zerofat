using Ardalis.Specification;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.Banners;
public class BannersBySearchRequestSpec : EntitiesByPaginationFilterSpec<Banner, BannerDto>
{
    public BannersBySearchRequestSpec(SearchBannersRequest request)
        : base(request)
    {
        Query.OrderBy(c => c.Index, !request.HasOrderBy())
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class BannerByIdSpec<T> : Specification<Banner, T>
{
    public BannerByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

public class LastBannerByIndexSpec : Specification<Banner>
{
    public LastBannerByIndexSpec() => Query.OrderByDescending(p => p.Index);
}

