using Ardalis.Specification;
using Mapster;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.MealRatings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.DailyMealSelections;

public class DailyMealSelectionsBySearchRequestSpec : EntitiesByPaginationFilterSpec<DailyMealSelection, DailyMealSelectionDto>
{
    public DailyMealSelectionsBySearchRequestSpec(SearchDailyMealSelectionsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
            .Where(x => x.Date == request.Date!.Value, request.Date.HasValue)
            .Where(x => x.ClientId == request.ClientId, request.ClientId.HasValue)
            .Where(x => x.ClientSubscriptionId == request.ClientSubscriptionId, request.ClientSubscriptionId.HasValue)
            .Where(x => x.MealPlanId == request.MealPlanId, request.MealPlanId.HasValue);
    }
}



public class SearchDailyMealSelectionsRequest : PaginationFilter, IQuery<PaginationResponse<DailyMealSelectionDto>>
{
    public DateOnly? Date { get; set; }
    public DefaultIdType? ClientId { get; set; }
    public DefaultIdType? ClientSubscriptionId { get; set; }
    public DefaultIdType? ClientLocationId { get; set; }
    public DefaultIdType? MealPlanId { get; set; }
}


public class SearchDailyMealSelectionsRequestHandler(
    IReadRepository<DailyMealSelection> repository,
    IReadRepository<Client> clientRepo,
    IReadRepository<CustomMeal> customMealRepo,
    IReadRepository<MealRating> mealRatingRepo,
    ICurrentUser currentUser) : IQueryHandler<SearchDailyMealSelectionsRequest, PaginationResponse<DailyMealSelectionDto>>
{
    private readonly IReadRepository<DailyMealSelection> _repository = repository;
    private readonly IReadRepository<Client> _clientRepo = clientRepo;
    private readonly IReadRepository<CustomMeal> _customMealRepo = customMealRepo;
    private readonly IReadRepository<MealRating> _mealRatingRepo = mealRatingRepo;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<PaginationResponse<DailyMealSelectionDto>> Handle(SearchDailyMealSelectionsRequest request, CancellationToken cancellationToken)
    {
        bool isClient = _currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (isClient)
        {
            request.ClientId = _currentUser.GetUserId();
        }

        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<DailyMealSelection, DailyMealSelectionDto>()
                .Map(destination => destination.Meal.Allergens, src => src.Meal.Allergens.Select(x => x.Allergen))
                .Map(destination => destination.Client, src => src.DailySelection.Client);

        var result = await _repository.PaginatedListAsync(new DailyMealSelectionsBySearchRequestSpec(request), request.PageNumber, request.PageSize, config, cancellationToken);

        if (result.Data.Count > 0)
        {
            var mealSelectionIds = result.Data.Select(x => x.Id).ToList();
            var ratings = await _mealRatingRepo.ListAsync(
                new ExpressionSpecification<MealRating>(x => mealSelectionIds.Contains(x.DailyMealSelectionId)),
                cancellationToken);
            var ratingsByMealSelectionId = ratings.ToDictionary(x => x.DailyMealSelectionId);

            foreach (var item in result.Data)
            {
                if (ratingsByMealSelectionId.TryGetValue(item.Id, out var rating))
                {
                    item.Rating = MealRatingHelper.ToSummaryDto(rating);
                }
            }
        }

        if (isClient)
        {
            var client = await _clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == _currentUser.GetUserId()), cancellationToken);
            foreach(var item in result.Data)
            {
                if(item.Meal.Allergens != null)
                {
                    item.Meal.Allergens.ForEach(x => x.IsAllergic = client.ClientAllergicIds.Contains(x.Id));
                }

                if (item.CustomMealId.HasValue)
                {
                    item.CustomMeal = await _customMealRepo.FirstOrDefaultAsync(new ExpressionSpecificationProjecting<CustomMeal, CustomMealSimplifyDto>(x=> x.Id == item.CustomMealId.Value), cancellationToken);
                }
            }
        }

        return result;
    }
}
