using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.MenuPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenuMeals;
public class AddMealsToMenuRequest : ICommand<Result<DefaultIdType>>
{
    public List<DefaultIdType>? MealIds { get; set; } // Foreign key to the recipe
    public DefaultIdType? MenuId { get; set; } // Foreign key to the recipe
    public DefaultIdType? MealTypeId { get; set; } // Foreign key to the recipe
    public DefaultIdType? MealPlanId { get; set; } // Foreign key to the recipe
    public DateOnly? Date { get; set; } // Foreign key to the recipe
}


public class AddMealsToMenuRequestValidator : CustomValidator<AddMealsToMenuRequest>
{
    public AddMealsToMenuRequestValidator(
        IReadRepository<MealType> mealTypeRepo,
        IReadRepository<Meal> mealRepo,
        IReadRepository<DailyMenuMeal> dailyMenuMealRepo,
        IReadRepository<MealPlan> mealPlanRepo,
        IStringLocalizer<AddMealsToMenuRequestValidator> localizer)
    {

        RuleFor(x => x.MealIds)
            .NotNull()
            .NotEmpty().WithMessage(localizer["One meal is required."]);

        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage(localizer["Menu is required."]);

        RuleFor(x => x.MealTypeId)
            .NotEmpty().WithMessage(localizer["Meal type is required."]);

        RuleFor(x => x.MealPlanId)
            .NotEmpty().WithMessage(localizer["Meal plan is required."]);

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage(localizer["Date is required."]);

        RuleFor(x => x.MealTypeId)
            .MustAsync(async (id, ct) => await mealTypeRepo.GetByIdAsync(id!.Value, ct) != null)
            .When(x => x.MealTypeId.HasValue)
            .WithMessage(localizer["Invalid meal type."]);

        RuleFor(x => x.MealPlanId)
            .MustAsync(async (id, ct) => await mealPlanRepo.GetByIdAsync(id!.Value, ct) != null)
            .When(x => x.MealPlanId.HasValue)
            .WithMessage(localizer["Invalid meal plan."]);

    }
}


public class AddMealsToMenuRequestHandler(
    IRepositoryWithEvents<DailyMenuMeal> repository,
    IRepositoryWithEvents<DailyMenu> dailyMenuRepo,
    IRepositoryWithEvents<Menu> menuRepo,
    IRepositoryWithEvents<MealPlanMealType> mealPlanMealTypeRepo) : ICommandHandler<AddMealsToMenuRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<DailyMenuMeal> _repository = repository;
    private readonly IRepositoryWithEvents<DailyMenu> _dailyMenuRepo = dailyMenuRepo;
    private readonly IRepositoryWithEvents<MealPlanMealType> _mealPlanMealTypeRepo = mealPlanMealTypeRepo;
    private readonly IRepositoryWithEvents<Menu> _menuRepo = menuRepo;

    public async Task<Result<DefaultIdType>> Handle(AddMealsToMenuRequest request, CancellationToken cancellationToken)
    {
        Menu? menu = await _menuRepo.GetByIdAsync(request.MenuId!.Value, cancellationToken);
        _ = menu ?? throw new NotFoundException("Menu not found");

        var date = request.Date!.Value;

        DailyMenu? dailyMenu = await _dailyMenuRepo.FirstOrDefaultAsync(new ExpressionSpecification<DailyMenu>(x => x.MenuId == request.MenuId && x.Date == date && x.MealTypeId == request.MealTypeId && x.MealPlanId == request.MealPlanId), cancellationToken);
        if(dailyMenu != null)
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
                await _repository.AddRangeAsync(list, withSaveChanges: true, cancellationToken: cancellationToken);
            }

            return await Result<DefaultIdType>.SuccessAsync(dailyMenu.Id);
        }
        else
        {
            var mealPlanMealType = await _mealPlanMealTypeRepo.FirstOrDefaultAsync(new ExpressionSpecification<MealPlanMealType>(x => x.MealPlanId == request.MealPlanId && x.MealTypeId == request.MealTypeId), cancellationToken);
            _ = mealPlanMealType ?? throw new BadRequestException("Meal Type and Meal Plan not supported in menu.");

            dailyMenu = new DailyMenu()
            {
                MealPlanId = request.MealPlanId,
                MealTypeId = request.MealTypeId,
                MenuId = request.MenuId,
                Date = date,
                AverageCalories = mealPlanMealType.AverageCalories,
                Price = mealPlanMealType.Price,
                Meals = request.MealIds!.ConvertAll(mealId => new DailyMenuMeal() { MealId = mealId })
            };

            dailyMenu.Meals.FirstOrDefault()!.IsDefault = true;

            await _dailyMenuRepo.AddAsync(dailyMenu, withSaveChanges: true, cancellationToken: cancellationToken);

            return await Result<DefaultIdType>.SuccessAsync(dailyMenu.Id);

        }

    }

}
