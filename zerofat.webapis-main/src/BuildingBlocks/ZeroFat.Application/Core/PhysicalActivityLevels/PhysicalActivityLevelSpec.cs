using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Models;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.PhysicalActivityLevels;
public class PhysicalActivityLevelsBySearchRequestSpec : EntitiesByPaginationFilterSpec<PhysicalActivityLevel, PhysicalActivityLevelDto>
{
    public PhysicalActivityLevelsBySearchRequestSpec(SearchPhysicalActivityLevelsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class PhysicalActivityLevelByIdSpec<T> : Specification<PhysicalActivityLevel, T>
{
    public PhysicalActivityLevelByIdSpec(Guid id) => Query.Where(p => p.Id == id);
}

