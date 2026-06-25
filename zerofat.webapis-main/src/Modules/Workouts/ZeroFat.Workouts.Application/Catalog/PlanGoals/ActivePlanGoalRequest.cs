using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Catalog.PlanGoals;
public class ActivePlanGoalRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActivePlanGoalRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class ActivePlanGoalRequestHandler(IRepository<PlanGoal> repository, IStringLocalizer<ActivePlanGoalRequestHandler> localizer) : ICommandHandler<ActivePlanGoalRequest, Result>
{
    private readonly IRepository<PlanGoal> _repository = repository;
    private readonly IStringLocalizer<ActivePlanGoalRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActivePlanGoalRequest request, CancellationToken cancellationToken)
    {
        var goal = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = goal ?? throw new NotFoundException(_localizer["Goal not found"]);

        goal.IsActive = !goal.IsActive;

        await _repository.UpdateAsync(goal, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
