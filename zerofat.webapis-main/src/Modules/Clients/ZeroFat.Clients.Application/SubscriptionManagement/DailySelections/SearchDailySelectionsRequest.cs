using Mapster;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.DailySelections;
public class SearchDailySelectionsRequest : PaginationFilter, IQuery<PaginationResponse<DailySelectionDto>>
{
    public DateOnly? Date { get; set; }
    public DefaultIdType? ClientId { get; set; }
    public DefaultIdType? ClientSubscriptionId { get; set; }
    public DefaultIdType? ClientLocationId { get; set; }
    public DefaultIdType? MealPlanId { get; set; }
}


public class SearchDailySelectionsRequestHandler(IReadRepository<DailySelection> repository) : IQueryHandler<SearchDailySelectionsRequest, PaginationResponse<DailySelectionDto>>
{
    private readonly IReadRepository<DailySelection> _repository = repository;

    public async Task<PaginationResponse<DailySelectionDto>> Handle(SearchDailySelectionsRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<DailySelection, DailySelectionDto>();
                // .Map(destination => destination.MealTypes, src => src.DailySelectionMealTypes.Select(x => x.MealType));

        return await _repository.PaginatedListAsync(new DailySelectionsBySearchRequestSpec(request), request.PageNumber, request.PageSize, config, cancellationToken);
    }
}
