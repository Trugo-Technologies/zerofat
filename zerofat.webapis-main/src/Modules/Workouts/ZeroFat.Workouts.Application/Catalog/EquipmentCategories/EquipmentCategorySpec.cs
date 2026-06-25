using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.EquipmentCategories;
public class EquipmentCategoriesBySearchRequestSpec : EntitiesByPaginationFilterSpec<EquipmentCategory, EquipmentCategoryDto>
{
    public EquipmentCategoriesBySearchRequestSpec(SearchEquipmentCategoriesRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class EquipmentCategoryByIdSpec<T> : Specification<EquipmentCategory, T>
{
    public EquipmentCategoryByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

