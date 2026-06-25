using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.SubscriptionPlans;

public class GetSubscriptionPlanRequest(DefaultIdType id) : IQuery<Result<SubscriptionPlanDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetSubscriptionPlanRequestHandler(IReadRepository<SubscriptionPlan> repository, IStringLocalizer<GetSubscriptionPlanRequestHandler> localizer) : IRequestHandler<GetSubscriptionPlanRequest, Result<SubscriptionPlanDetailsDto>>
{
    private readonly IReadRepository<SubscriptionPlan> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<SubscriptionPlanDetailsDto>> Handle(GetSubscriptionPlanRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new SubscriptionPlanByIdSpec<SubscriptionPlanDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Subscription plan not found", request.Id]);

        return await Result<SubscriptionPlanDetailsDto>.SuccessAsync(entity);
    }

}
