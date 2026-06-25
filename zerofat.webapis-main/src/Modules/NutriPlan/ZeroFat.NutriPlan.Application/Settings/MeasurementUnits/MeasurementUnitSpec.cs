using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Application.MeasurementUnits;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MeasurementUnits;
public class MeasurementUnitsBySearchRequestSpec : EntitiesByPaginationFilterSpec<MeasurementUnit, MeasurementUnitDto>
{
    public MeasurementUnitsBySearchRequestSpec(SearchMeasurementUnitsRequest request)
        : base(request)
    {
        Query
            .Where(x => x.IngredientMeasurementUnits.Any(x => x.IngredientId == request.IngredientId), request.IngredientId.HasValue)
            .Where(x => !x.IsDefault)
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class MeasurementUnitByIdSpec<T> : Specification<MeasurementUnit, T>
{
    public MeasurementUnitByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

public class MeasurementUnitByCodeSpec<T> : Specification<MeasurementUnit, T>
{
    public MeasurementUnitByCodeSpec(string code)
    {
        Query.Where(p => p.Code == code);
    }
}
