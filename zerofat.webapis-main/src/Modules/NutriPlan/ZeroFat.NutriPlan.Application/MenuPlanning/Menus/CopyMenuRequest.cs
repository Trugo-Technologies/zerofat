using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenuMeals;
using ZeroFat.NutriPlan.Application.Settings.MealPlans;
using ZeroFat.NutriPlan.Domain.MenuPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.Menus;
public class CopyMenuRequest : ICommand<Result<DefaultIdType>>
{
    public string? NameEn { get; set; } // Name of the menu (e.g., "Weekly Low Carb Plan")
    public string? NameAr { get; set; } // Name of the menu (e.g., "Weekly Low Carb Plan")
    public bool IsPublished { get; set; } // Indicates if the menu is published or not
    public DateOnly StartDate { get; set; } // Start date of the menu
    public Guid? MenuId { get; set; } // Start date of the menu
}


public class CopyMenuRequestValidator : CustomValidator<CopyMenuRequest>
{
    public CopyMenuRequestValidator(
        IReadRepository<Menu> repository,
        IReadRepository<Category> categoryRepo,
        IReadRepository<MealType> mealTypeRepo,
        IStringLocalizer<CopyMenuRequestValidator> localaizer)
    {

    }
}


public class CopyMenuRequestHandler(
    IRepositoryWithEvents<Menu> repository,
    IRepositoryWithEvents<MealPlan> mealPlanRepo,
    IRepositoryWithEvents<DailyMenuMeal> dailyMenuMealRepo,
    IFileStorageManager fileStorageManager) : ICommandHandler<CopyMenuRequest, Result<DefaultIdType>>
{
    public async Task<Result<DefaultIdType>> Handle(CopyMenuRequest request, CancellationToken cancellationToken)
    {
        var oldMenu = await repository.GetByIdAsync(request.MenuId, cancellationToken);
        if (oldMenu == null)
            throw new BadRequestException("Invalid Menu ID");

        int days = oldMenu.EndDate!.Value.DayNumber - oldMenu.StartDate.DayNumber;

        var menu = new Menu
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            IsPublished = request.IsPublished,
            StartDate = request.StartDate,
            EndDate = request.StartDate.AddDays(days)
        };

        // get all meal plans
        var mealPlans = await mealPlanRepo.ListAsync(new MealPlansSpec(), cancellationToken);

        // calculate date offset between new and old menu
        int dateOffset = menu.StartDate.DayNumber - oldMenu.StartDate.DayNumber;

        for (DateOnly newDate = menu.StartDate; newDate <= menu.EndDate; newDate = newDate.AddDays(1))
        {
            // map back to old date
            var oldDate = newDate.AddDays(-dateOffset);

            foreach (var mealPlan in mealPlans)
            {
                foreach (var mealType in mealPlan.MealPlanMealTypes)
                {
                    var meals = await dailyMenuMealRepo.ListAsync(
                        new CopyDailyMenuMealSpec(oldMenu.Id, mealPlan.Id, mealType.MealTypeId, oldDate),
                        cancellationToken);

                    menu.DailyMenus.Add(new DailyMenu()
                    {
                        Date = newDate, // keep new date
                        MealTypeId = mealType.MealTypeId,
                        MealPlanId = mealPlan.Id,
                        Price = mealType.Price,
                        AverageCalories = mealType.AverageCalories,
                        Meals = meals
                            .DistinctBy(x => x.MealId)
                            .Select(x => new DailyMenuMeal()
                            {
                                IsDefault = x.IsDefault,
                                MealId = x.MealId,
                            })
                             .ToList()
                    });
                }
            }
        }


        await repository.AddAsync(menu, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(menu.Id);
    }

}
