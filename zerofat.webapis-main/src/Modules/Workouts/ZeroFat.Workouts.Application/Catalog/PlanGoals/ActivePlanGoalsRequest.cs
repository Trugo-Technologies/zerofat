using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Catalog.PlanGoals;
public class ActivePlanGoalsRequest : ICommand<Result>
{
    public List<DefaultIdType> Ids { get; set; } = [];
    public bool IsActive { get; set; }
}

public class ActivePlanGoalsRequestHandler(IRepository<PlanGoal> repository, IStringLocalizer<ActivePlanGoalsRequestHandler> localizer) : ICommandHandler<ActivePlanGoalsRequest, Result>
{

    public async Task<Result> Handle(ActivePlanGoalsRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var planGoal = await repository.GetByIdAsync(ingId, cancellationToken);
            if (planGoal != null)
            {
                planGoal.IsActive = request.IsActive;
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
