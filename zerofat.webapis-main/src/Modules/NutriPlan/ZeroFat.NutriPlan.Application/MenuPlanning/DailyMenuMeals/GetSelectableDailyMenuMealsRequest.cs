using Mapster;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Shared;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Application.MealPlanning.CustomMeals;
using ZeroFat.NutriPlan.Application.MealPlanning.Meals;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.MenuPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenuMeals;
public class GetSelectableDailyMenuMealsRequest : IQuery<Result<List<SelecteabeleMealPlanMenuDto>>>
{
    public DefaultIdType MealTypeId { get; set; }
    public DateOnly Date { get; set; }
}

public class GetSelectableDailyMenuMealsRequestHandler(
    IReadRepository<DailyMenuMeal> repository,
    IReadRepository<CustomMeal> customMealRepo,
    IClientService clientService,
    ICurrentUser currentUser,
    IReadRepository<MealPlan> mealPlanRepo) :
    IQueryHandler<GetSelectableDailyMenuMealsRequest, Result<List<SelecteabeleMealPlanMenuDto>>>
{
    public async Task<Result<List<SelecteabeleMealPlanMenuDto>>> Handle(GetSelectableDailyMenuMealsRequest request, CancellationToken cancellationToken)
    {
        ValidateClientRole();

        var config = CreateMappingConfig();

        List<SelecteabeleMealPlanMenuDto> mealPlans = await mealPlanRepo.ListAsync(new ExpressionSpecificationProjecting<MealPlan, SelecteabeleMealPlanMenuDto>(x => x.IsActive), cancellationToken);

        List<MenuMealDetailsDto> meals = await repository.ListAsync(new ExpressionSpecificationProjecting<DailyMenuMeal, MenuMealDetailsDto>(x => x.DailyMenu.Date == request.Date && x.DailyMenu.MealTypeId == request.MealTypeId), config, cancellationToken);

        if (meals.Count != 0)
        {
            var clientAllergicIds = await clientService.GetClientAllergicIdsByClientId(currentUser.GetUserId());
            if (clientAllergicIds?.Count > 0)
            {
                foreach (var meal in meals.Where(m => m.Meal?.Allergens != null))
                {
                    foreach (var allergen in meal.Meal!.Allergens)
                    {
                        allergen.IsAllergic = clientAllergicIds.Contains(allergen.Id);
                    }
                }
            }

        }

        foreach (SelecteabeleMealPlanMenuDto mealPlan in mealPlans)
        {
            mealPlan.Meals = meals.Where(x => x.MealPlanId == mealPlan.Id).ToList();
        }

        await AddCustomMealsIfAny(mealPlans, config, cancellationToken);

        return await Result<List<SelecteabeleMealPlanMenuDto>>.SuccessAsync(mealPlans.Where(x => x.Meals.Count > 0 || x.CustomMeals.Count > 0).ToList());
    }

    // Helper methods
    private void ValidateClientRole()
    {
        bool isClient = currentUser.GetRoleType()!.Equals(
            nameof(UserType.Client),
            StringComparison.OrdinalIgnoreCase);

        if (!isClient)
        {
            throw new BadRequestException("Only clients can access this feature");
        }
    }

    private TypeAdapterConfig CreateMappingConfig()
    {
        var config = new TypeAdapterConfig();

        config.NewConfig<DailyMenuMeal, MenuMealDetailsDto>()
            .Map(dest => dest.MealPlanId, src => src.DailyMenu.MealPlanId)
            .Map(dest => dest.DefaultPrice, src => src.DailyMenu.Price)
            .Map(dest => dest.Meal.Allergens, src => src.Meal.Allergens.Select(x => x.Allergen));

        config.NewConfig<Meal, MealDto>()
            .Map(dest => dest.Allergens, src => src.Allergens.Select(x => x.Allergen));

        return config;
    }

    private async Task AddCustomMealsIfAny(
        List<SelecteabeleMealPlanMenuDto> mealPlans,
        TypeAdapterConfig config,
        CancellationToken cancellationToken)
    {
        var customMeals = await customMealRepo.ListAsync(
        new ExpressionSpecificationProjecting<CustomMeal, CustomMealSimplifyDto>(
            x => x.ClientId == currentUser.GetUserId()),
        config,
        cancellationToken);

        if (customMeals.Count == 0) return;

        mealPlans.Add(new SelecteabeleMealPlanMenuDto
        {
            NameEn = "My Meals",
            NameAr = "My Meals",
            Type = MealSelectionType.Custom,
            CustomMeals = customMeals
        });
    }
}


