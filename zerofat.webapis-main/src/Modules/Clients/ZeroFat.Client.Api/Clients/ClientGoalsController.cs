using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.ClientManagement.ClientGoals;
using ZeroFat.ClientPortal.Application.Contracts;

namespace ZeroFat.ClientPortal.Api.Clients;

internal sealed class ClientGoalsController : BaseController
{
    private readonly IClientPortalModule _clientModule;

    public ClientGoalsController(IClientPortalModule clientModule)
    {
        _clientModule = clientModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<ClientGoalSimplifyDto>> SearchAsync(SearchClientGoalsRequest request)
        => await _clientModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateClientGoalRequest request)
        => await _clientModule.ExecuteCommandAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<ClientGoalDetailsDto>> GetAsync(DefaultIdType id)
        => await _clientModule.ExecuteQueryAsync(new GetClientGoalRequest(id));

    [HttpGet("current")]
    public async Task<Result<ClientGoalDetailsDto>> GetCurrentAsync()
       => await _clientModule.ExecuteQueryAsync(new GetCurrentClientGoalRequest());

}
