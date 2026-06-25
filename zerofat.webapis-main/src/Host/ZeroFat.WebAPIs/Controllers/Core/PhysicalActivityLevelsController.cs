using Microsoft.AspNetCore.Mvc;
using ZeroFat.Api.Controllers;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Core.Contracts;
using ZeroFat.Application.Core.PhysicalActivityLevels;

namespace ZeroFat.WebAPIs.Controllers.Core;

internal sealed class PhysicalActivityLevelsController : BaseController
{
    private readonly ICoreModule _coreModule;

    public PhysicalActivityLevelsController(ICoreModule coreModule) => _coreModule = coreModule;

    [HttpPost("search")]
    public async Task<PaginationResponse<PhysicalActivityLevelDto>> SearchAsync(SearchPhysicalActivityLevelsRequest request)
        => await _coreModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreatePhysicalActivityLevelRequest request)
        => await _coreModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdatePhysicalActivityLevelRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _coreModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _coreModule.ExecuteCommandAsync(new DeletePhysicalActivityLevelRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(Guid id)
       => await _coreModule.ExecuteCommandAsync(new ActivePhysicalActivityLevelRequest(id));

    [HttpGet("{id:guid}")]
    public async Task<Result<PhysicalActivityLevelDetailsDto>> GetAsync(DefaultIdType id)
     => await _coreModule.ExecuteQueryAsync(new GetPhysicalActivityLevelRequest(id));

}
