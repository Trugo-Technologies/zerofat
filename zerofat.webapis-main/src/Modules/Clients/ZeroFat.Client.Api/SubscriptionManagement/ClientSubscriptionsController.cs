using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;

namespace ZeroFat.ClientPortal.Api.SubscriptionManagement;

internal sealed class ClientSubscriptionsController : BaseController
{
    private readonly IClientPortalModule _clientPortalModule;

    public ClientSubscriptionsController(IClientPortalModule clientPortalModule)
    {
        _clientPortalModule = clientPortalModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<ClientSubscriptionDto>> SearchAsync(SearchClientSubscriptionsRequest request)
        => await _clientPortalModule.ExecuteQueryAsync(request);

    [HttpPost("subscribe")]
    public async Task<Result<ClientSubscriptionSimplifyDto>> CreateAsync(CreateClientSubscriptionRequest request)
        => await _clientPortalModule.ExecuteCommandAsync(request);

    [HttpPost("update-info")]
    public async Task<Result<ClientSubscriptionSimplifyDto>> CreateAsync(UpdateClientSubscriptionInfoRequest request)
        => await _clientPortalModule.ExecuteCommandAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<ClientSubscriptionDetailsDto>> GetAsync(DefaultIdType id)
        => await _clientPortalModule.ExecuteQueryAsync(new GetClientSubscriptionRequest(id));

    [HttpGet("check-coupon")]
    public async Task<Result<StripeCouponDto>> GetCouponCodeAsync([FromQuery] string couponCode)
       => await _clientPortalModule.ExecuteQueryAsync(new GetStripCouponByCodeRequest(couponCode));

    [HttpPost("renew-subscribe")]
    public async Task<Result> RenewSubscriptionAsync(RenewClientSubscriptionRequest request) => await _clientPortalModule.ExecuteCommandAsync(request);
}
