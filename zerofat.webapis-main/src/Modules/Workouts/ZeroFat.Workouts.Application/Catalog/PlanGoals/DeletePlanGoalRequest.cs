using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.PlanGoals;

public class DeletePlanGoalRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeletePlanGoalRequest(DefaultIdType id)
    {
        Id = id;
    }
}


public class DeletePlanGoalRequestHandler(IRepository<PlanGoal> repository, IStringLocalizer<DeletePlanGoalRequestHandler> localizer, IReadRepository<Plan> planRepo) : IRequestHandler<DeletePlanGoalRequest, Result<DefaultIdType>>
{
    private readonly IRepository<PlanGoal> _repository = repository;
    private readonly IReadRepository<Plan> _planRepo = planRepo;
    private readonly IStringLocalizer<DeletePlanGoalRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeletePlanGoalRequest request, CancellationToken cancellationToken)
    {
        var goal = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = goal ?? throw new NotFoundException(_localizer["Goal not found"]);

        bool used = await _planRepo.AnyAsync(new ExpressionSpecification<Plan>(x => x.PlanGoalId == goal.Id), cancellationToken);
        if (used)
            throw new BadRequestException(_localizer["Can not be deleted, because it's linked with plans"]);

        await _repository.DeleteAsync(goal, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(goal.Id);
    }

}
