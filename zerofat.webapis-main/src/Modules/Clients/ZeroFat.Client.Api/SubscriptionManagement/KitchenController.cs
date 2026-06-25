using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Infrastructure.Kitchen;


namespace ZeroFat.ClientPortal.Api.SubscriptionManagement;

internal sealed class KitchenController : BaseController
{
    private readonly IClientPortalModule _clientPortalModule;

    public KitchenController(IClientPortalModule clientPortalModule)
    {
        _clientPortalModule = clientPortalModule;
    }

    [HttpPost("meal-count")]
    public async Task<Result<List<MealRequestReport>>> SearchAsync(SearchMealByCountRequest request)
        => await _clientPortalModule.ExecuteQueryAsync(request);

    [HttpPost("ordered-meal")]
    public async Task<PaginationResponse<OrderedMealDto>> SearchAsync(SearchOrderedMealsRequest request)
       => await _clientPortalModule.ExecuteQueryAsync(request);
}
