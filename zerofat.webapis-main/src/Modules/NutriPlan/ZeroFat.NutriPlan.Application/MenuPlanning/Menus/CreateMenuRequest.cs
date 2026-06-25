using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.NutriPlan.Application.Settings.MealPlans;
using ZeroFat.NutriPlan.Domain.MenuPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.Menus;
public class CreateMenuRequest : ICommand<Result<DefaultIdType>>
{
    public string? NameEn { get; set; } // Name of the menu (e.g., "Weekly Low Carb Plan")
    public string? NameAr { get; set; } // Name of the menu (e.g., "Weekly Low Carb Plan")
    public bool IsPublished { get; set; } // Indicates if the menu is published or not
    public DateOnly StartDate { get; set; } // Start date of the menu
    public DateOnly? EndDate { get; set; } // End date of the menu (only for weekly menus)
    // public List<Guid>? MealTypeIds { get; set; } // End date of the menu (only for weekly menus)
    // public List<Guid>? MealPlanIds { get; set; } // End date of the menu (only for weekly menus)
}


public class CreateMenuRequestValidator : CustomValidator<CreateMenuRequest>
{
    public CreateMenuRequestValidator(
        IReadRepository<Menu> repository,
        IReadRepository<Category> categoryRepo,
        IReadRepository<MealType> mealTypeRepo,
        IStringLocalizer<CreateMenuRequestValidator> localaizer)
    {

        When(x => x.EndDate.HasValue, () =>
        {
            RuleFor(u => u.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        });

    }
}


public class CreateMenuRequestHandler(
    IRepositoryWithEvents<Menu> repository,
    IRepositoryWithEvents<MealPlan> mealPlanRepo) : ICommandHandler<CreateMenuRequest, Result<DefaultIdType>>
{
    public async Task<Result<DefaultIdType>> Handle(CreateMenuRequest request, CancellationToken cancellationToken)
    {
        var menu = new Menu
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            IsPublished = request.IsPublished,
            StartDate = request.StartDate,
            EndDate = request.EndDate ?? request.StartDate
        };

        var mealPlans = await mealPlanRepo.ListAsync(new MealPlansSpec(), cancellationToken);
        for (DateOnly date = menu.StartDate; date <= menu.EndDate; date = date.AddDays(1))
        {
            foreach (var mealPlan in mealPlans)
            {
                foreach (var mealType in mealPlan.MealPlanMealTypes)
                {
                    menu.DailyMenus.Add(new DailyMenu()
                    {
                        Date = date,
                        MealTypeId = mealType.MealTypeId,
                        MealPlanId = mealPlan.Id,
                        Price = mealType.Price,
                        AverageCalories = mealType.AverageCalories,
                    });
                }
            }
        }

        await repository.AddAsync(menu, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(menu.Id);
    }

}
