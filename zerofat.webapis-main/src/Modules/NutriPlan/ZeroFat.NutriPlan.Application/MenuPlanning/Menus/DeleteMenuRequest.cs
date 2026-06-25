using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.Menus;

public class DeleteMenuRequest : ICommand<Result<Guid>>
{
    public Guid Id { get; set; }
    public DeleteMenuRequest(Guid id) => Id = id;
}


public class DeleteMenuRequestHandler(
    IRepository<Menu> repository,
    IRepository<DailyMenu> dailyMenuRepo,
    IRepository<DailyMenuMeal> dailyMenuMealMenuRepo,
    IStringLocalizer<DeleteMenuRequestHandler> localizer) : IRequestHandler<DeleteMenuRequest, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(DeleteMenuRequest request, CancellationToken cancellationToken)
    {
        var menu = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = menu ?? throw new NotFoundException(localizer["Menu not found"]);

        var dailyMenuMeals = await dailyMenuMealMenuRepo.ListAsync(new ExpressionSpecification<DailyMenuMeal>(x => x.DailyMenu.MenuId == menu.Id), cancellationToken);
        if (dailyMenuMeals.Count != 0)
            await dailyMenuMealMenuRepo.DeleteRangeAsync(dailyMenuMeals, cancellationToken);

        var dailyMenus = await dailyMenuRepo.ListAsync(new ExpressionSpecification<DailyMenu>(x => x.MenuId == menu.Id), cancellationToken);
        if (dailyMenus.Count != 0)
            await dailyMenuRepo.DeleteRangeAsync(dailyMenus, cancellationToken);

        await repository.DeleteAsync(menu, cancellationToken);

        return await Result<Guid>.SuccessAsync(menu.Id);
    }

}
