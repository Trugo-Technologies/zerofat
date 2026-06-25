using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.ClientManagement.ClientChats;
using ZeroFat.ClientPortal.Application.Contracts;

namespace ZeroFat.ClientPortal.Api.Clients;

[AllowAnonymous]
internal sealed class ClientChatsController : BaseController
{
    private readonly IClientPortalModule _clientModule;

    public ClientChatsController(IClientPortalModule clientModule)
    {
        _clientModule = clientModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<ClientChatDto>> SearchAsync(SearchClientChatsRequest request)
        => await _clientModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateClientChatRequest request)
        => await _clientModule.ExecuteCommandAsync(request);


    // [HttpPut("{id}")]
    // public async Task<ActionResult<Result>> UpdateAsync(UpdateClientChatRequest request, DefaultIdType id)
    //    => request.Id != id ? BadRequest() :
    //         await _clientModule.ExecuteCommandAsync(request);

}
