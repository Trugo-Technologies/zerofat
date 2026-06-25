using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.NutriPlan.Domain.MenuPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenuMeals;
public class AddMultiMealsToMenuRequest : ICommand<Result<DefaultIdType>>
{
    public List<DefaultIdType>? MealIds { get; set; } // Foreign key to the recipe
    public DefaultIdType? MenuId { get; set; } // Foreign key to the recipe
    public List<AddMealPlanAndMealTypes> MealPlanIds { get; set; } = []; // Foreign key to the recipe
    public List<DateOnly>? Dates { get; set; } // Foreign key to the recipe
}

public class AddMealPlanAndMealTypes
{
    public DefaultIdType MealPlanId { get; set; }
    public List<DefaultIdType> MealTypeIds { get; set; } = [];
}


public class AddMultiMealsToMenuRequestValidator : CustomValidator<AddMultiMealsToMenuRequest>
{
    public AddMultiMealsToMenuRequestValidator(
        IStringLocalizer<AddMultiMealsToMenuRequestValidator> localizer)
    {
        RuleFor(x => x.Dates)
            .NotEmpty().NotNull()
            .WithMessage(localizer["One date is required."]);

        RuleFor(x => x.MealIds)
            .NotNull()
            .NotEmpty().WithMessage(localizer["One meal is required."]);

        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage(localizer["Menu is required."]);

    }
}


public class AddMultiMealsToMenuRequestHandler(
    IRepositoryWithEvents<DailyMenuMeal> repository,
    IRepositoryWithEvents<DailyMenu> dailyMenuRepo,
    IRepositoryWithEvents<Menu> menuRepo,
    IRepositoryWithEvents<MealPlanMealType> mealPlanMealTypeRepo) : ICommandHandler<AddMultiMealsToMenuRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<DailyMenuMeal> _repository = repository;
    private readonly IRepositoryWithEvents<DailyMenu> _dailyMenuRepo = dailyMenuRepo;
    private readonly IRepositoryWithEvents<MealPlanMealType> _mealPlanMealTypeRepo = mealPlanMealTypeRepo;
    private readonly IRepositoryWithEvents<Menu> _menuRepo = menuRepo;

    public async Task<Result<DefaultIdType>> Handle(AddMultiMealsToMenuRequest request, CancellationToken cancellationToken)
    {
        Menu? menu = await _menuRepo.GetByIdAsync(request.MenuId!.Value, cancellationToken);
        _ = menu ?? throw new NotFoundException("Menu not found");

        foreach(var date in request.Dates)
        {
            if (menu.StartDate > date || menu.EndDate < date)
                continue;
            foreach(var mealPlan in request.MealPlanIds)
            {
                foreach(var mealTypeId in mealPlan.MealTypeIds)
                {
                    DailyMenu? dailyMenu = await _dailyMenuRepo.FirstOrDefaultAsync(new ExpressionSpecification<DailyMenu>(x => x.MenuId == request.MenuId && x.Date == date && x.MealTypeId == mealTypeId && x.MealPlanId == mealPlan.MealPlanId), cancellationToken);
                    if (dailyMenu != null)
                    {
                        var list = new List<DailyMenuMeal>();
                        DailyMenuMeal? old = await _repository.FirstOrDefaultAsync(new ExpressionSpecification<DailyMenuMeal>(x => x.IsDefault && x.DailyMenuId == dailyMenu.Id), cancellationToken);

                        foreach (var mealId in request.MealIds!)
                        {
                            bool alreadyExsit = await _repository.AnyAsync(new ExpressionSpecification<DailyMenuMeal>(x => x.DailyMenuId == dailyMenu.Id && x.MealId == mealId), cancellationToken);
                            if (!alreadyExsit)
                            {
                                list.Add(new()
                                {
                                    MealId = mealId,
                                    DailyMenuId = dailyMenu.Id,
                                    IsDefault = false,
                                });
                            }
                        }
                        if (old == null && list.Count > 0)
                        {
                            list.FirstOrDefault()!.IsDefault = true;
                        }

                        if (list.Count > 0)
                        {
                            await _repository.AddRangeAsync(list, withSaveChanges: false, cancellationToken: cancellationToken);
                        }

                    }
                    else
                    {
                        var mealPlanMealType = await _mealPlanMealTypeRepo.FirstOrDefaultAsync(new ExpressionSpecification<MealPlanMealType>(x => x.MealPlanId == mealPlan.MealPlanId && x.MealTypeId == mealTypeId), cancellationToken);
                        _ = mealPlanMealType ?? throw new BadRequestException("Meal Type and Meal Plan not supported in menu.");

                        dailyMenu = new DailyMenu()
                        {
                            MealPlanId = mealPlan.MealPlanId,
                            MealTypeId = mealTypeId,
                            MenuId = request.MenuId,
                            Date = date,
                            AverageCalories = mealPlanMealType.AverageCalories,
                            Price = mealPlanMealType.Price,
                            Meals = request.MealIds!.ConvertAll(mealId => new DailyMenuMeal() { MealId = mealId })
                        };

                        dailyMenu.Meals.FirstOrDefault()!.IsDefault = true;

                        await _dailyMenuRepo.AddAsync(dailyMenu, withSaveChanges: false, cancellationToken: cancellationToken);

                    }
                }

            }

        }

        await _repository.SaveChangesAsync(cancellationToken);
        await _dailyMenuRepo.SaveChangesAsync(cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(menu.Id);

    }

}
