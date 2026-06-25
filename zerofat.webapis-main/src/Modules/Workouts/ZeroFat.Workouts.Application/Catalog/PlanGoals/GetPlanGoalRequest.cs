using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Catalog.PlanGoals;
public class GetPlanGoalRequest(DefaultIdType id) : IQuery<Result<PlanGoalDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetPlanGoalRequestHandler(IRepositoryWithEvents<PlanGoal> repository, IStringLocalizer<GetPlanGoalRequestHandler> localizer) : IRequestHandler<GetPlanGoalRequest, Result<PlanGoalDetailsDto>>
{
    private readonly IRepositoryWithEvents<PlanGoal> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<PlanGoalDetailsDto>> Handle(GetPlanGoalRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new PlanGoalByIdSpec<PlanGoalDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["PlanGoal not found", request.Id]);

        return await Result<PlanGoalDetailsDto>.SuccessAsync(entity);
    }

}
