using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;
using ZeroFat.ClientPortal.Domain.Discounts;

namespace ZeroFat.ClientPortal.Application.Discounts.DiscountRules;
public class ActiveDiscountRulesRequest : ICommand<Result>
{
    public List<DefaultIdType> Ids { get; set; } = [];
    public bool IsActive { get; set; }
}

public class ActiveDiscountRulesRequestHandler(
    IRepository<DiscountRule> repository,
    IStringLocalizer<ActiveDiscountRulesRequestHandler> localizer) : ICommandHandler<ActiveDiscountRulesRequest, Result>
{

    public async Task<Result> Handle(ActiveDiscountRulesRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var discountRule = await repository.GetByIdAsync(ingId, cancellationToken);
            if (discountRule != null)
            {
                discountRule.IsActive = request.IsActive;
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
