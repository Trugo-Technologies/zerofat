using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.Equipments;
public class EquipmentsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Equipment, EquipmentDto>
{
    public EquipmentsBySearchRequestSpec(SearchEquipmentsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.CategoryId == request.CategoryId, request.CategoryId.HasValue)
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class EquipmentByIdSpec<T> : Specification<Equipment, T>
{
    public EquipmentByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

