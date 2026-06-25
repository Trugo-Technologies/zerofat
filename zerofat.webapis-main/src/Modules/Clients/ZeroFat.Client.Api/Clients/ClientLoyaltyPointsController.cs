using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.ClientManagement.ClientLoyaltyPoints;
using ZeroFat.ClientPortal.Application.Contracts;

namespace ZeroFat.ClientPortal.Api.Clients;

internal sealed class ClientLoyaltyPointsController : BaseController
{
    private readonly IClientPortalModule _clientModule;

    public ClientLoyaltyPointsController(IClientPortalModule clientModule)
    {
        _clientModule = clientModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<ClientLoyaltyPointDto>> SearchAsync(SearchClientLoyaltyPointsRequest request)
        => await _clientModule.ExecuteQueryAsync(request);

}
