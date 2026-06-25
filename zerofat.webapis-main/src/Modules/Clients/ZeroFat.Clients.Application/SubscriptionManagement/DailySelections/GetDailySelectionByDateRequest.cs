using Mapster;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.DailyMealSelections;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.MenuPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.DailySelections;
public class GetDailySelectionByDateRequest : IQuery<Result<DailySelectionDetailsDto>>
{
    public DateOnly Date { get; set; }
}


public class GetDailySelectionByDateRequestHandler(
    IReadRepository<DailySelection> repository,
    IReadRepository<MealType> mealTypesRepo,
    IReadRepository<DailyMealSelection> dailyMealSelectionRepo,
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<CustomMeal> customMealRepo,
    IClientPortalSettingservice clientPortal,
    IStringLocalizer<GetDailySelectionByDateRequestHandler> localizer) : IQueryHandler<GetDailySelectionByDateRequest, Result<DailySelectionDetailsDto>>
{

    public async Task<Result<DailySelectionDetailsDto>> Handle(GetDailySelectionByDateRequest request, CancellationToken cancellationToken)
    {
        bool isClient = currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (!isClient)
        {
            throw new ForbiddenException("not client");
        }

        var client = await clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == currentUser.GetUserId()), cancellationToken);
        if (client == null)
        {
            return await Result<DailySelectionDetailsDto>.SuccessAsync(data: null);
        }

        var enabled = await clientPortal.GetEnableDailyMealSelections();
        if (!enabled)
        {
            return await Result<DailySelectionDetailsDto>.SuccessAsync(data: null);
        }

        DailySelectionDetailsDto? entity = await repository.FirstOrDefaultAsync(new DailySelectionByDateSpec<DailySelectionDetailsDto>(request.Date, client!.Id), cancellationToken);
        if (entity == null)
        {
            return await Result<DailySelectionDetailsDto>.SuccessAsync(data: null);
        }

        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<DailyMealSelection, MealSelectionDto>()
        .Map(destination => destination.Meal.Allergens, src => src.Meal.Allergens.Select(x => x.Allergen));

        List<MealSelectionDto> mealsSelections = await dailyMealSelectionRepo.ListAsync(new ExpressionSpecificationProjecting<DailyMealSelection, MealSelectionDto>(x => x.DailySelectionId == entity.Id && x.ClientId == client!.Id), config, cancellationToken);

        foreach (IGrouping<DefaultIdType?, MealSelectionDto> item in mealsSelections.GroupBy(x => x.MealTypeId))
        {
            MealType? mealType = item.Key.HasValue ? await mealTypesRepo.GetByIdAsync(item.Key.Value, cancellationToken) : null;
            DailySelectionMealTypeDto dailySelectionMealTypeDto = new()
            {
                Index = mealType?.Index,
                NameAr = mealType?.NameAr ?? "Add-On",
                NameEn = mealType?.NameEn ?? "Add-On",
                DailyMealSelections = [.. item.ToList()]
            };

            foreach (var dailyMealSelection in dailySelectionMealTypeDto.DailyMealSelections)
            {
                if (dailyMealSelection.Meal?.Allergens != null)
                {
                    dailyMealSelection.Meal.Allergens.ForEach(x => x.IsAllergic = client.ClientAllergicIds.Contains(x.Id));
                }

                if (dailyMealSelection.CustomMealId.HasValue)
                {
                    dailyMealSelection.CustomMeal = await customMealRepo.FirstOrDefaultAsync(new ExpressionSpecificationProjecting<CustomMeal, CustomMealSimplifyDto>(x => x.Id == dailyMealSelection.CustomMealId.Value), cancellationToken);
                }
            } 
            entity.MealTypes.Add(dailySelectionMealTypeDto);
        }

        entity.MealTypes = entity.MealTypes.OrderBy(x => x.Index).ToList();
        return await Result<DailySelectionDetailsDto>.SuccessAsync(entity);
    }

}
