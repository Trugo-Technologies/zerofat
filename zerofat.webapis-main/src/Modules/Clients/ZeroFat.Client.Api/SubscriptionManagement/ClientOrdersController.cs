using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientOrders;

namespace ZeroFat.ClientPortal.Api.SubscriptionManagement;

internal sealed class ClientOrdersController : BaseController
{
    private readonly IClientPortalModule _clientPortalModule;

    public ClientOrdersController(IClientPortalModule clientPortalModule)
    {
        _clientPortalModule = clientPortalModule;
    }

    [HttpPost("order")]
    public async Task<Result<ClientOrderDto>> CreateAsync(CreateClientOrderRequest request)
        => await _clientPortalModule.ExecuteCommandAsync(request);

}
