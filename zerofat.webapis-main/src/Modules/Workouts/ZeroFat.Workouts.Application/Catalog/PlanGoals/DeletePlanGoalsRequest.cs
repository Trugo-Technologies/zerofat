using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.PlanGoals;
public class DeletePlanGoalsRequest : ICommand<Result<bool>>
{
    public List<DefaultIdType> Ids { get; set; } = [];
}

public class DeletePlanGoalsRequestHandler(
    IRepositoryWithEvents<PlanGoal> repository,
    IStringLocalizer<DeletePlanGoalsRequestHandler> localizer,
    IReadRepository<Plan> planRepo) : IRequestHandler<DeletePlanGoalsRequest, Result<bool>>
{

    public async Task<Result<bool>> Handle(DeletePlanGoalsRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var plan = await repository.GetByIdAsync(ingId, cancellationToken);
            if (plan != null)
            {
                if (await planRepo.AnyAsync(new ExpressionSpecification<Plan>(x => x.PlanGoalId == plan.Id), cancellationToken))
                    continue;

                await repository.DeleteAsync(plan, withSaveChanges: false, cancellationToken: cancellationToken);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return await Result<bool>.SuccessAsync(data: true);
    }

}
