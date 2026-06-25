using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.Discounts;

namespace ZeroFat.ClientPortal.Application.Discounts.DiscountRules;
public class DeleteDiscountRuleRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteDiscountRuleRequest(DefaultIdType id) => Id = id;
}


public class DeleteDiscountRuleRequestHandler(
    IRepository<DiscountRule> repository,
    IStringLocalizer<DeleteDiscountRuleRequestHandler> localizer
    ) : IRequestHandler<DeleteDiscountRuleRequest, Result<DefaultIdType>>
{

    public async Task<Result<DefaultIdType>> Handle(DeleteDiscountRuleRequest request, CancellationToken cancellationToken)
    {
        var discountRule = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = discountRule ?? throw new NotFoundException(localizer["DiscountRule not found"]);

        if (discountRule.RedemptionsUsed > 0)
        {
            throw new BadRequestException(localizer["Can not be deleted, because the DiscountRule is used."]);
        }

        await repository.DeleteAsync(discountRule, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(discountRule.Id);
    }

}
