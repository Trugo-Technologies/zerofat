using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealPlans;
public class ActiveMealPlanRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveMealPlanRequest(DefaultIdType id) => Id = id;
}

public class ActiveMealPlanRequestHandler(IRepository<MealPlan> repository, IStringLocalizer<ActiveMealPlanRequestHandler> localizer) : ICommandHandler<ActiveMealPlanRequest, Result>
{
    private readonly IRepository<MealPlan> _repository = repository;
    private readonly IStringLocalizer<ActiveMealPlanRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveMealPlanRequest request, CancellationToken cancellationToken)
    {
        var MealPlan = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = MealPlan ?? throw new NotFoundException(_localizer["MealPlan not found"]);

        MealPlan.IsActive = !MealPlan.IsActive;

        await _repository.UpdateAsync(MealPlan, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
