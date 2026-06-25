using Mapster;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Application.ClientManagement.Clients;
using ZeroFat.ClientPortal.Application.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.DailySelections;
using Ardalis.Specification;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.CQRS;

namespace ZeroFat.ClientPortal.Infrastructure.Kitchen;
public class SearchOrderedMealsRequest : PaginationFilter, IQuery<PaginationResponse<OrderedMealDto>>
{
    public DateOnly? Date { get; set; }
    public DefaultIdType? ClientId { get; set; }
    public DefaultIdType? ClientSubscriptionId { get; set; }
    public DefaultIdType? ClientLocationId { get; set; }
    public DefaultIdType? MealPlanId { get; set; }
}


public class SearchOrderedMealsRequestHandler(IReadRepository<DailySelection> repository) : IQueryHandler<SearchOrderedMealsRequest, PaginationResponse<OrderedMealDto>>
{
    private readonly IReadRepository<DailySelection> _repository = repository;

    public async Task<PaginationResponse<OrderedMealDto>> Handle(SearchOrderedMealsRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<DailySelection, OrderedMealDto>()
                .Map(destination => destination.Meals, src => src.DailyMealSelections.Where(x => x.MealId.HasValue).Select(x => x.Meal));

        return await _repository.PaginatedListAsync(new OrderedMealsBySearchRequestSpec(request), request.PageNumber, request.PageSize, config, cancellationToken);
    }
}

public class OrderedMealDto : IDto
{
    public DefaultIdType Id { get; set; }

    public DateOnly Date { get; set; } // Date of the meal selection
    public string? Notes { get; set; } // Special notes or preferences for the meal

    public PreferredMealTime DeliveryTime { get; set; } // The client's preferred delivery time for this meal
    public double TotalCalories { get; set; } // Total calories in the selected meal
    public double TotalFats { get; set; } // Total fat content in the selected meal
    public double TotalCarbohydrates { get; set; } // Total carbohydrate content in the selected meal
    public double TotalProteins { get; set; } // Total protein content in the selected meal

    public string? SpecialInstructions { get; set; } // Special instructions for the meal delivery

    public DateTime? DeliveryDate { get; set; } // Date of the actual delivery

    public ClientLocationSimplifyDto? ClientLocation { get; set; } // Foreign key to the client
    public ClientSimplifyDto? Client { get; set; } // Foreign key to the client
    public List<MealSimplifyDto> Meals { get; set; } // Foreign key to the client
}

public class OrderedMealsBySearchRequestSpec : EntitiesByPaginationFilterSpec<DailySelection, OrderedMealDto>
{
    public OrderedMealsBySearchRequestSpec(SearchOrderedMealsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
            .Where(x => x.Date == request.Date.Value, request.Date.HasValue)
            .Where(x => x.ClientId == request.ClientId, request.ClientId.HasValue)
            .Where(x => x.ClientSubscriptionId == request.ClientSubscriptionId, request.ClientSubscriptionId.HasValue)
            .Where(x => x.ClientLocationId == request.ClientLocationId, request.ClientLocationId.HasValue)
            .Where(x => x.DailySelectionStatus != DailySelectionStatus.Paused)
            .Where(x => x.MealPlanId == request.MealPlanId, request.MealPlanId.HasValue);
    }
}
