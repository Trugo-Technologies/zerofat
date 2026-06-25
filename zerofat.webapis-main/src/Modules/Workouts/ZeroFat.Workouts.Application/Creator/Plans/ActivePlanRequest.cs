using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Creator.Plans;
public class ActivePlanRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActivePlanRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class ActivePlanRequestHandler(IRepository<Plan> repository, IStringLocalizer<ActivePlanRequestHandler> localizer) : ICommandHandler<ActivePlanRequest, Result>
{
    private readonly IRepository<Plan> _repository = repository;
    private readonly IStringLocalizer<ActivePlanRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActivePlanRequest request, CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = plan ?? throw new NotFoundException(_localizer["Plan not found"]);


        plan.IsActive = !plan.IsActive;

        await _repository.UpdateAsync(plan, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
