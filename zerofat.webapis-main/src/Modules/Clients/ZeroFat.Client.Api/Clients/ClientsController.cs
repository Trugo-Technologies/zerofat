using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.ClientManagement.Clients;
using ZeroFat.ClientPortal.Application.Contracts;


namespace ZeroFat.ClientPortal.Api.Clients;

internal sealed class ClientsController : BaseController
{
    private readonly IClientPortalModule _clientModule;

    public ClientsController(IClientPortalModule clientModule)
    {
        _clientModule = clientModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<ClientDto>> SearchAsync(SearchClientsRequest request)
        => await _clientModule.ExecuteQueryAsync(request);

    [HttpPost("register")]
    public async Task<Result> CreateAsync(RegisterClientRequest request)
        => await _clientModule.ExecuteCommandAsync(request);

    [HttpPost("setup-intent")]
    public async Task<Result> CreateAsync(CreatePaymentMethodOnStripe request)
        => await _clientModule.ExecuteCommandAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<ClientDetailsDto>> GetAsync(DefaultIdType id)
        => await _clientModule.ExecuteQueryAsync(new GetClientRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(DefaultIdType id)
       => await _clientModule.ExecuteCommandAsync(new ActiveClientRequest(id));

    [HttpPut("setup-name")]
    public async Task<Result> SetupClientNameRequestAsync([FromForm] SetupClientNameRequest req)
      => await _clientModule.ExecuteCommandAsync(req);

    [HttpPut("setup-allergics")]
    public async Task<Result> SetupClientAllergicsRequestAsync(SetupClientAllergicsRequest req)
      => await _clientModule.ExecuteCommandAsync(req);
}
