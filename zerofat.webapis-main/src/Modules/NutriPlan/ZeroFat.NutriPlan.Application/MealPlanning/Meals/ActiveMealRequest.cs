using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Meals;
public class ActiveMealRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveMealRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class ActiveMealRequestHandler(IRepository<Meal> repository, IStringLocalizer<ActiveMealRequestHandler> localizer) : ICommandHandler<ActiveMealRequest, Result>
{
    private readonly IRepository<Meal> _repository = repository;
    private readonly IStringLocalizer<ActiveMealRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveMealRequest request, CancellationToken cancellationToken)
    {
        var meal = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = meal ?? throw new NotFoundException(_localizer["Meal not found"]);

        meal.IsActive = !meal.IsActive;

        await _repository.UpdateAsync(meal, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
