using Mapster;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.DailySelections;
public class GetDailySelectionRequest : IQuery<Result<DailySelectionDetailsDto>>
{
    public DefaultIdType Id { get; set; }
    public GetDailySelectionRequest(DefaultIdType id)
    {
        Id = id;
    }
}


public class GetDailySelectionRequestHandler(
    IReadRepository<DailySelection> repository,
    IReadRepository<MealType> mealTypesRepo,
    IReadRepository<DailyMealSelection> dailyMealSelectionRepo) : IQueryHandler<GetDailySelectionRequest, Result<DailySelectionDetailsDto>>
{
    private readonly IReadRepository<DailySelection> _repository = repository;
    private readonly IReadRepository<DailyMealSelection> _dailyMealSelectionRepo = dailyMealSelectionRepo;
    private readonly IReadRepository<MealType> _mealTypesRepo = mealTypesRepo;

    public async Task<Result<DailySelectionDetailsDto>> Handle(GetDailySelectionRequest request, CancellationToken cancellationToken)
    {
        DailySelectionDetailsDto? entity = await _repository.FirstOrDefaultAsync(new DailySelectionByIdSpec<DailySelectionDetailsDto>(request.Id), cancellationToken);
        if (entity == null)
        {
            return await Result<DailySelectionDetailsDto>.SuccessAsync(data: null);
        }

        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<DailyMealSelection, MealSelectionDto>()
        .Map(destination => destination.Meal.Allergens, src => src.Meal.Allergens.Select(x => x.Allergen));



        List<MealSelectionDto> mealsSelections = await _dailyMealSelectionRepo.ListAsync(new ExpressionSpecificationProjecting<DailyMealSelection, MealSelectionDto>(x => x.DailySelectionId == entity.Id), config, cancellationToken);

        foreach (IGrouping<DefaultIdType?, MealSelectionDto> item in mealsSelections.GroupBy(x => x.MealTypeId))
        {
            MealType? mealType = null;
            if (item.Key.HasValue)
            {
                mealType = await _mealTypesRepo.GetByIdAsync(item.Key, cancellationToken);
            }
            DailySelectionMealTypeDto dailySelectionMealTypeDto = new()
            {
                Index = mealType?.Index ?? 0,
                NameAr = mealType?.NameAr ?? "Add-On",
                NameEn = mealType?.NameEn ?? "Add-On",
                DailyMealSelections = [.. item]
            };

            entity.MealTypes.Add(dailySelectionMealTypeDto);
        }

        return await Result<DailySelectionDetailsDto>.SuccessAsync(entity);
    }

}
