using ZeroFat.Application.Common.Models;
using ZeroFat.Domain.Enums;
using ZeroFat.Infrastructure.Api.Swagger;
using ZeroFat.Users.Application.Authentication.Commands;
using ZeroFat.Users.Application.Authentication.DTOs;
using ZeroFat.Users.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ZeroFat.Users.Api.Controllers;
internal sealed class AuthenticationController : BaseController
{
    private readonly IUserModule _userModule;
    public AuthenticationController(IUserModule userModule) => _userModule = userModule;

    [AllowAnonymous]
    [HttpPost("send-otp")]
    [DeviceHeader("DeviceModel", typeof(string), "Device Model", true, "Windows 10")]
    [DeviceHeader("DeviceType", typeof(DeviceType), "Device Type", true, DeviceType.Web)]
    [DeviceHeader("DeviceOs", typeof(string), "Device Os", true, "Windows")]
    [DeviceHeader("Platform", typeof(string), "Platform", false, "ERP CMS")]
    [DeviceHeader("Version", typeof(string), "Version", false, "V 1.0")]
    [DeviceHeader("BaseDeviceId", typeof(string), "Base Device Id", true, "3c212d88-cd4a-4593-a4b0-65859fb175a5")]
    public async Task<ActionResult<Result<string>>> GenerateOTPAsync(GetOtpTokenRequest request)
    {
        request.Headers = GetDeviceHedearsParam();
        return await _userModule.ExecuteCommandAsync(request);
    }

    [AllowAnonymous]
    [HttpPost("send-client-otp")]
    [DeviceHeader("DeviceModel", typeof(string), "Device Model", true, "Windows 10")]
    [DeviceHeader("DeviceType", typeof(DeviceType), "Device Type", true, DeviceType.Web)]
    [DeviceHeader("DeviceOs", typeof(string), "Device Os", true, "Windows")]
    [DeviceHeader("Platform", typeof(string), "Platform", false, "ERP CMS")]
    [DeviceHeader("Version", typeof(string), "Version", false, "V 1.0")]
    [DeviceHeader("BaseDeviceId", typeof(string), "Base Device Id", true, "3c212d88-cd4a-4593-a4b0-65859fb175a5")]
    public async Task<ActionResult<Result<string>>> GenerateClientOTPAsync(GetClientOtpTokenRequest request)
    {
        request.Headers = GetDeviceHedearsParam();
        return await _userModule.ExecuteCommandAsync(request);
    }

    [AllowAnonymous]
    [HttpPost("verify-otp")]
    [DeviceHeader("DeviceModel", typeof(string), "Device Model", true, "Windows 10")]
    [DeviceHeader("DeviceType", typeof(DeviceType), "Device Type", true, DeviceType.Web)]
    [DeviceHeader("DeviceOs", typeof(string), "Device Os", true, "Windows")]
    [DeviceHeader("Platform", typeof(string), "Platform", false, "ERP CMS")]
    [DeviceHeader("Version", typeof(string), "Version", false, "V 1.0")]
    [DeviceHeader("BaseDeviceId", typeof(string), "Base Device Id", true, "3c212d88-cd4a-4593-a4b0-65859fb175a5")]
    public async Task<ActionResult<Result<TokenResponse>>> GenerateOTPAsync(GetTokenByOtpRequest request)
    {
        request.Headers = GetDeviceHedearsParam();
        return await _userModule.ExecuteCommandAsync(request);
    }


    [AllowAnonymous]
    [HttpPost("login")]
    [DeviceHeader("DeviceModel", typeof(string), "Device Model", true, "Windows 10")]
    [DeviceHeader("DeviceType", typeof(DeviceType), "Device Type", true, DeviceType.Web)]
    [DeviceHeader("DeviceOs", typeof(string), "Device Os", true, "Windows")]
    [DeviceHeader("Platform", typeof(string), "Platform", false, "ERP CMS")]
    [DeviceHeader("Version", typeof(string), "Version", false, "V 1.0")]
    [DeviceHeader("BaseDeviceId", typeof(string), "Base Device Id", true, "3c212d88-cd4a-4593-a4b0-65859fb175a5")]
    public async Task<ActionResult<Result<TokenResponse>>> LoginAsync(GetTokenRequest request)
    {
        request.Headers = GetDeviceHedearsParam();
        return await _userModule.ExecuteCommandAsync(request);
    }

    [AllowAnonymous]
    [HttpPost("2fa")]
    [DeviceHeader("DeviceModel", typeof(string), "Device Model", true, "Windows 10")]
    [DeviceHeader("DeviceType", typeof(DeviceType), "Device Type", true, DeviceType.Web)]
    [DeviceHeader("DeviceOs", typeof(string), "Device Os", true, "Windows")]
    [DeviceHeader("Platform", typeof(string), "Platform", false, "ERP CMS")]
    [DeviceHeader("Version", typeof(string), "Version", false, "V 1.0")]
    [DeviceHeader("BaseDeviceId", typeof(string), "Base Device Id", true, "3c212d88-cd4a-4593-a4b0-65859fb175a5")]
    public async Task<ActionResult<Result<TokenResponse>>> LoginAsync(GetTokenWith2FARequest request)
    {
        request.Headers = GetDeviceHedearsParam();
        return await _userModule.ExecuteCommandAsync(request);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [DeviceHeader("DeviceModel", typeof(string), "Device Model", true, "Windows 10")]
    [DeviceHeader("DeviceType", typeof(DeviceType), "Device Type", true, DeviceType.Web)]
    [DeviceHeader("DeviceOs", typeof(string), "Device Os", true, "Windows")]
    [DeviceHeader("Platform", typeof(string), "Platform", false, "ERP CMS")]
    [DeviceHeader("Version", typeof(string), "Version", false, "V 1.0")]
    [DeviceHeader("BaseDeviceId", typeof(string), "Base Device Id", true, "3c212d88-cd4a-4593-a4b0-65859fb175a5")]
    public async Task<Result<TokenResponse>> RefreshAsync(RefreshTokenRequest request)
    {
        request.Headers = GetDeviceHedearsParam();
        return await _userModule.ExecuteCommandAsync(request);
    }


    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<Result> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        return await _userModule.ExecuteCommandAsync(request);
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<Result> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        return await _userModule.ExecuteCommandAsync(request);
    }

    [HttpPost("me")]
    public async Task<Result> MeRequest([FromBody] GetMeUserRequest request)
    {
        return await _userModule.ExecuteCommandAsync(request);
    }

    private TokenRequestHeaders GetDeviceHedearsParam() =>
         new()
         {
             DeviceModel = Request.Headers["DeviceModel"],
             DeviceOs = Request.Headers["DeviceOs"],
             DeviceType = (DeviceType?)Enum.Parse(typeof(DeviceType), Request.Headers["DeviceType"]),
             Platform = Request.Headers["Platform"],
             Version = Request.Headers["Version"],
             Language = Request.Headers["Accept-Language"],
             BaseDeviceId = Request.Headers["BaseDeviceId"],
             IPAddress = Request.Headers.ContainsKey("X-Forwarded-For") ? Request.Headers["X-Forwarded-For"] : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A"
         };


    /// <summary>
    /// Initiates the account deletion process by sending a confirmation token.
    /// </summary>
    [HttpPost("delete-account")]
    public async Task<Result> DeleteAccount([FromBody] DeleteAccountRequest request)
    {
        return await _userModule.ExecuteCommandAsync(request);
    }

    /// <summary>
    /// Confirms and finalizes the account deletion using the provided token.
    /// </summary>
    [HttpPost("confirm-delete-account")]
    public async Task<Result> ConfirmDeleteAccount([FromBody] ConfirmDeleteAccountRequest request)
    {
        return await _userModule.ExecuteCommandAsync(request);
    }

}
