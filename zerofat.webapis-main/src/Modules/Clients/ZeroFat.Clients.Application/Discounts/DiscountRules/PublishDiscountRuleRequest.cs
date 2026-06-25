using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;
using ZeroFat.ClientPortal.Domain.Discounts;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.Discounts.DiscountRules;
public class PublishDiscountRuleRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public PublishDiscountRuleRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class PublishDiscountRuleRequestHandler(
    IRepository<DiscountRule> repository, 
    IStripeService stripeService,
    IStringLocalizer<PublishDiscountRuleRequestHandler> localizer) : ICommandHandler<PublishDiscountRuleRequest, Result>
{
    private readonly IRepository<DiscountRule> _repository = repository;
    private readonly IStringLocalizer<PublishDiscountRuleRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(PublishDiscountRuleRequest request, CancellationToken cancellationToken)
    {
        var discountRule = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = discountRule ?? throw new NotFoundException(_localizer["DiscountRule not found"]);

        if(discountRule.StripeId.HasValue())
            throw new BadRequestException(_localizer["DiscountRule already published"]);

        var duration = discountRule.Duration == DiscountDuration.Forever ? "forever" : discountRule.Duration == DiscountDuration.Once ? "once" : "repeating";

        discountRule.StripeId = await stripeService.CreateDiscountRuleOnStripeAsync(new StripeCouponDto()
        {
            Code = discountRule.Code,
            AmountOff = discountRule.AmountOff,
            Duration = duration,
            Name = discountRule.Code,
            PercentOff = discountRule.PercentOff,
            DurationInMonths = discountRule.DurationInMonths,
            MaxRedemptions = discountRule.MaxRedemptions,
            RedeemBy = discountRule.ExpirationDate?.ToDateTime(TimeOnly.MinValue),
        });

        await _repository.UpdateAsync(discountRule, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
