using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.SubscriptionPlans;
public class ActiveSubscriptionPlanRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveSubscriptionPlanRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class ActiveSubscriptionPlanRequestHandler(IRepository<SubscriptionPlan> repository, IStringLocalizer<ActiveSubscriptionPlanRequestHandler> localizer) : ICommandHandler<ActiveSubscriptionPlanRequest, Result>
{
    private readonly IRepository<SubscriptionPlan> _repository = repository;
    private readonly IStringLocalizer<ActiveSubscriptionPlanRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveSubscriptionPlanRequest request, CancellationToken cancellationToken)
    {
        var subscriptionPlan = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = subscriptionPlan ?? throw new NotFoundException(_localizer["Subscription plan not found"]);

        subscriptionPlan.IsActive = !subscriptionPlan.IsActive;

        await _repository.UpdateAsync(subscriptionPlan, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
