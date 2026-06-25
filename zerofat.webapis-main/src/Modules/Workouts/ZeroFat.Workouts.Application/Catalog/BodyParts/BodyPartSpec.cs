using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.BodyParts;
public class BodyPartsBySearchRequestSpec : EntitiesByPaginationFilterSpec<BodyPart, BodyPartDto>
{
    public BodyPartsBySearchRequestSpec(SearchBodyPartsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class BodyPartByIdSpec<T> : Specification<BodyPart, T>
{
    public BodyPartByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}
