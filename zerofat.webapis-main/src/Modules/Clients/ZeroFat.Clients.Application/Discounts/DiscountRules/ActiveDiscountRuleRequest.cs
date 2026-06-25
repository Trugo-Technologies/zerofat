using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;
using ZeroFat.ClientPortal.Domain.Discounts;

namespace ZeroFat.ClientPortal.Application.Discounts.DiscountRules;
public class ActiveDiscountRuleRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveDiscountRuleRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class ActiveDiscountRuleRequestHandler(IRepository<DiscountRule> repository, IStringLocalizer<ActiveDiscountRuleRequestHandler> localizer) : ICommandHandler<ActiveDiscountRuleRequest, Result>
{
    private readonly IRepository<DiscountRule> _repository = repository;
    private readonly IStringLocalizer<ActiveDiscountRuleRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveDiscountRuleRequest request, CancellationToken cancellationToken)
    {
        var discountRule = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = discountRule ?? throw new NotFoundException(_localizer["DiscountRule not found"]);


        discountRule.IsActive = !discountRule.IsActive;

        await _repository.UpdateAsync(discountRule, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
