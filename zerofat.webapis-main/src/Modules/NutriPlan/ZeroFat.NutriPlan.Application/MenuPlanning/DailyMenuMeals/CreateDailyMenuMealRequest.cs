using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.MenuPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenuMeals;
public class CreateDailyMenuMealRequest : ICommand<Result<DefaultIdType>>
{
    public bool IsDefault { get; set; }

    public DefaultIdType? MealId { get; set; } // Foreign key to the recipe
    public DefaultIdType? MenuId { get; set; } // Foreign key to the recipe
    public DefaultIdType? MealTypeId { get; set; } // Foreign key to the recipe
    public DefaultIdType? MealPlanId { get; set; } // Foreign key to the recipe
    public DateOnly? Date { get; set; } // Foreign key to the recipe
}


public class CreateDailyMenuMealRequestValidator : CustomValidator<CreateDailyMenuMealRequest>
{
    public CreateDailyMenuMealRequestValidator(
        IReadRepository<MealType> mealTypeRepo,
        IReadRepository<Meal> mealRepo,
        IReadRepository<DailyMenuMeal> dailyMenuMealRepo,
        IReadRepository<MealPlan> mealPlanRepo,
        IStringLocalizer<CreateDailyMenuMealRequestValidator> localizer)
    {
        RuleFor(x => x.MealId)
           .NotEmpty().WithMessage(localizer["Meal is required."]);

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

        RuleFor(x => x.MealId)
            .MustAsync(async (id, ct) => await mealRepo.GetByIdAsync(id!.Value, ct) != null)
            .When(x => x.MealId.HasValue)
            .WithMessage(localizer["Invalid meal."]);
    }
}


public class CreateDailyMenuMealRequestHandler(
    IRepositoryWithEvents<DailyMenuMeal> repository,
    IRepositoryWithEvents<DailyMenu> dailyMenuRepo,
    IRepositoryWithEvents<Menu> menuRepo,
    IRepositoryWithEvents<MealPlanMealType> mealPlanMealTypeRepo) : ICommandHandler<CreateDailyMenuMealRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<DailyMenuMeal> _repository = repository;
    private readonly IRepositoryWithEvents<DailyMenu> _dailyMenuRepo = dailyMenuRepo;
    private readonly IRepositoryWithEvents<MealPlanMealType> _mealPlanMealTypeRepo = mealPlanMealTypeRepo;
    private readonly IRepositoryWithEvents<Menu> _menuRepo = menuRepo;

    public async Task<Result<DefaultIdType>> Handle(CreateDailyMenuMealRequest request, CancellationToken cancellationToken)
    {
        Menu? menu = await _menuRepo.GetByIdAsync(request.MenuId!.Value, cancellationToken);
        _ = menu ?? throw new NotFoundException("Menu not found");

        var date = request.Date!.Value;

        DailyMenu? dailyMenu = await _dailyMenuRepo.FirstOrDefaultAsync(new ExpressionSpecification<DailyMenu>(x => x.MenuId == request.MenuId && x.Date == date && x.MealTypeId == request.MealTypeId && x.MealPlanId == request.MealPlanId), cancellationToken);
        if(dailyMenu != null)
        {
            bool alreadyExsit = await _repository.AnyAsync(new ExpressionSpecification<DailyMenuMeal>(x => x.DailyMenuId == dailyMenu.Id && x.MealId == request.MealId), cancellationToken);
            if(alreadyExsit)
                throw new BadRequestException("Meal Already Exsit in Menu.");

            DailyMenuMeal entity = new()
            {
                MealId = request.MealId,
                DailyMenuId = dailyMenu.Id,
                IsDefault = request.IsDefault,
            };

            DailyMenuMeal? old = await _repository.FirstOrDefaultAsync(new ExpressionSpecification<DailyMenuMeal>(x => x.IsDefault && x.DailyMenuId == dailyMenu.Id), cancellationToken);

            if (entity.IsDefault && old != null)
            {
                old.IsDefault = false;
            }
            else
            {
                entity.IsDefault = true;
            }

            await _repository.AddAsync(entity, withSaveChanges: true, cancellationToken: cancellationToken);

            return await Result<DefaultIdType>.SuccessAsync(entity.Id);
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
                Meals = [new() { MealId = request.MealId, IsDefault = request.IsDefault}]
            };

            await _dailyMenuRepo.AddAsync(dailyMenu, withSaveChanges: true, cancellationToken: cancellationToken);

            return await Result<DefaultIdType>.SuccessAsync(dailyMenu.Id);

        }

    }

}
