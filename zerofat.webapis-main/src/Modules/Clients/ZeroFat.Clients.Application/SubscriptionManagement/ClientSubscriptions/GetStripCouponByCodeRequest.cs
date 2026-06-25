using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;

public class GetStripCouponByCodeRequest(string? couponCode) : IQuery<Result<StripeCouponDto>>
{
    public string CouponCode { get; set; } = couponCode;
}

public class GetStripCouponByCodeRequestHandler(
    IStripeService stripeService,
    IStringLocalizer<GetStripCouponByCodeRequestHandler> localizer) : IRequestHandler<GetStripCouponByCodeRequest, Result<StripeCouponDto>>
{
    private readonly IStripeService _stripeService = stripeService;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<StripeCouponDto>> Handle(GetStripCouponByCodeRequest request, CancellationToken cancellationToken)
    {
        var coupon = await _stripeService.GetCouponByCodeAsync(request.CouponCode);

        return await Result<StripeCouponDto>.SuccessAsync(data: coupon);
    }

}
