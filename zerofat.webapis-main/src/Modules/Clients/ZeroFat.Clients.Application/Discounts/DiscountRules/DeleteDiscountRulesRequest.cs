using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.Discounts;

namespace ZeroFat.ClientPortal.Application.Discounts.DiscountRules;
public class DeleteDiscountRulesRequest : ICommand<Result<bool>>
{
    public List<DefaultIdType> Ids { get; set; } = [];
}

public class DeleteDiscountRulesRequestHandler(
    IRepositoryWithEvents<DiscountRule> repository,
    IStringLocalizer<DeleteDiscountRulesRequestHandler> localizer) : IRequestHandler<DeleteDiscountRulesRequest, Result<bool>>
{

    public async Task<Result<bool>> Handle(DeleteDiscountRulesRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var discountRule = await repository.GetByIdAsync(ingId, cancellationToken);
            if (discountRule != null)
            {
                if (discountRule.RedemptionsUsed > 0)
                {
                    continue;
                }

                await repository.DeleteAsync(discountRule, withSaveChanges: false, cancellationToken: cancellationToken);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return await Result<bool>.SuccessAsync(data: true);
    }

}
