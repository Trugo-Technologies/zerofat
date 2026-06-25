using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.NutrientsAttributes;
public class NutrientsAttributesBySearchRequestSpec : EntitiesByPaginationFilterSpec<NutrientsAttribute, NutrientsAttributeDto>
{
    public NutrientsAttributesBySearchRequestSpec(SearchNutrientsAttributesRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class NutrientsAttributeByIdSpec<T> : Specification<NutrientsAttribute, T>
{
    public NutrientsAttributeByIdSpec(Guid id) => Query.Where(p => p.Id == id);
}

