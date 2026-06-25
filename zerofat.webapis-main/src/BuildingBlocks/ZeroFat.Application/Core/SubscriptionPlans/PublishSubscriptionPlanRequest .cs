using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.SubscriptionPlans;
public class PublishSubscriptionPlanRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public PublishSubscriptionPlanRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class PublishSubscriptionPlanRequestHandler(
    IRepository<SubscriptionPlan> repository,
    IStringLocalizer<PublishSubscriptionPlanRequestHandler> localizer,
    IStripeService stripeService) : ICommandHandler<PublishSubscriptionPlanRequest, Result>
{
    private readonly IRepository<SubscriptionPlan> _repository = repository;
    private readonly IStripeService _stripeService = stripeService;
    private readonly IStringLocalizer<PublishSubscriptionPlanRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(PublishSubscriptionPlanRequest request, CancellationToken cancellationToken)
    {
        var subscriptionPlan = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = subscriptionPlan ?? throw new NotFoundException(_localizer["Subscription plan not found"]);

        // if (subscriptionPlan.StripeId.HasValue())
        // {
        //     throw new BadRequestException(_localizer["The plan already published on stripe."]);
        // }
        // 
        // var paymobId = await _stripeService.CreateSubscriptionLink((int)subscriptionPlan.SubscriptionType, subscriptionPlan.TitleEn);
        // if(!paymobId.HasValue())
        //     throw new BadRequestException(_localizer["Something went wrong"]);
        // 
        // subscriptionPlan.PaymobId = paymobId;

        // await _repository.UpdateAsync(subscriptionPlan, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
