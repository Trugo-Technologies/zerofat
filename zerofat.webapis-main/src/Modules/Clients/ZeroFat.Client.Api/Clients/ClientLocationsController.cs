using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.ClientManagement;
using ZeroFat.ClientPortal.Application.ClientManagement.ClientLocations;
using ZeroFat.ClientPortal.Application.Contracts;

namespace ZeroFat.ClientPortal.Api.Clients;

internal sealed class ClientLocationsController : BaseController
{
    private readonly IClientPortalModule _clientModule;

    public ClientLocationsController(IClientPortalModule clientModule)
    {
        _clientModule = clientModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<ClientLocationSimplifyDto>> SearchAsync(SearchClientLocationsRequest request)
        => await _clientModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateClientLocationRequest request)
        => await _clientModule.ExecuteCommandAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<ClientLocationDetailsDto>> GetAsync(DefaultIdType id)
        => await _clientModule.ExecuteQueryAsync(new GetClientLocationRequest(id));

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync(UpdateClientLocationRequest request, DefaultIdType id)
       => request.Id != id ? BadRequest() :
            await _clientModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _clientModule.ExecuteCommandAsync(new DeleteClientLocationRequest(id));

}
