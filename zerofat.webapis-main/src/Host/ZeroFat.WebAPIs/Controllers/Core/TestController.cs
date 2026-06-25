using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Api.Controllers;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Core.FAQs;

namespace ZeroFat.WebAPIs.Controllers.Core;
[AllowAnonymous]
internal sealed class TestController : BaseController
{
    private readonly IPaymobService _paymobService;

    public TestController(IPaymobService paymobService) => _paymobService = paymobService;

    [HttpPost("search")]
    public async Task SearchAsync()
    {
        await _paymobService.test("request");
    }
}
