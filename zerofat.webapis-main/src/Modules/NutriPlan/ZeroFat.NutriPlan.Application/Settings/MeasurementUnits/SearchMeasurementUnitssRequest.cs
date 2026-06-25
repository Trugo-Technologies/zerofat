using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Application.MeasurementUnits;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MeasurementUnits;
public class SearchMeasurementUnitsRequest : PaginationFilter, IQuery<PaginationResponse<MeasurementUnitDto>>
{
    public DefaultIdType? IngredientId { get; set; }
}


public class SearchMeasurementUnitsRequestHandler(IReadRepository<MeasurementUnit> repository) : IQueryHandler<SearchMeasurementUnitsRequest, PaginationResponse<MeasurementUnitDto>>
{
    private readonly IReadRepository<MeasurementUnit> _repository = repository;

    public async Task<PaginationResponse<MeasurementUnitDto>> Handle(SearchMeasurementUnitsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new MeasurementUnitsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
