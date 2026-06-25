using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.Discounts;

namespace ZeroFat.ClientPortal.Application.Discounts.DiscountRules;
public class GetDiscountRuleRequest(DefaultIdType id) : IQuery<Result<DiscountRuleDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetDiscountRuleRequestHandler(
    IReadRepository<DiscountRule> repository,
    IStringLocalizer<GetDiscountRuleRequestHandler> localizer) : IRequestHandler<GetDiscountRuleRequest, Result<DiscountRuleDetailsDto>>
{

    public async Task<Result<DiscountRuleDetailsDto>> Handle(GetDiscountRuleRequest request, CancellationToken cancellationToken)
    {

        var entity = await repository.FirstOrDefaultAsync(new DiscountRuleByIdSpec<DiscountRuleDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(localizer["DiscountRule not found", request.Id]);

        return await Result<DiscountRuleDetailsDto>.SuccessAsync(entity);
    }

}
