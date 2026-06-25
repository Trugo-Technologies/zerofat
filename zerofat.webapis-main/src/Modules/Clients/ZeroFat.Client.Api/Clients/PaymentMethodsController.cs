using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.ClientManagement.ClientPaymentMethods;
using ZeroFat.ClientPortal.Application.Contracts;


namespace ZeroFat.ClientPortal.Api.Clients;

internal class ClientPaymentMethodsController : BaseController
{
    private readonly IClientPortalModule _clientModule;
    public ClientPaymentMethodsController(IClientPortalModule clientModule)
    {
        _clientModule = clientModule;
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<ClientPaymentMethodDto>>> SearchAsync(SearchClientPaymentMethodsRequest request)
    {
        return await _clientModule.ExecuteQueryAsync(request);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Result<ClientPaymentMethodDetailsDto>>> GetAsync(DefaultIdType id)
    {
        var request = new GetClientPaymentMethodRequest(id);
        return await _clientModule.ExecuteQueryAsync(request);
    }

    [HttpPost]
    public async Task<Result<DefaultIdType>> CreateAsync(CreateClientPaymentMethodRequest request)
       => await _clientModule.ExecuteCommandAsync(request);


    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<DefaultIdType>>> DeleteAsync(DefaultIdType id)
        => await _clientModule.ExecuteCommandAsync(new DeleteClientPaymentMethodRequest(id));

    [HttpPut("default/{id:guid}")]
    public async Task<ActionResult<Result<DefaultIdType>>> SetDefaultAsync(SetDefaultPaymentMethodRequest request, DefaultIdType id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await _clientModule.ExecuteCommandAsync(request));
    }
}
