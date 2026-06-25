using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.SubscriptionPlans;
public class UpdateSubscriptionPlanRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public bool? PercentageDiscount { get; set; }
    public double? DiscountAmount { get; set; }
    public bool IsActive { get; set; }
}


public class UpdateSubscriptionPlanRequestHandler(IRepository<SubscriptionPlan> repository, IStringLocalizer<UpdateSubscriptionPlanRequestHandler> localizer) : IRequestHandler<UpdateSubscriptionPlanRequest, Result<DefaultIdType>>
{
    private readonly IRepository<SubscriptionPlan> _repository = repository;
    private readonly IStringLocalizer<UpdateSubscriptionPlanRequestHandler> _localizer = localizer;


    public async Task<Result<DefaultIdType>> Handle(UpdateSubscriptionPlanRequest request, CancellationToken cancellationToken)
    {
        var subscriptionPlan = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);
        _ = subscriptionPlan ?? throw new NotFoundException(_localizer["Subscription plan not found"]);

        subscriptionPlan.DiscountAmount = request.DiscountAmount;
        subscriptionPlan.PercentageDiscount = request.PercentageDiscount;
        subscriptionPlan.IsActive = request.IsActive;


        await _repository.UpdateAsync(subscriptionPlan, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(subscriptionPlan.Id);
    }
}

