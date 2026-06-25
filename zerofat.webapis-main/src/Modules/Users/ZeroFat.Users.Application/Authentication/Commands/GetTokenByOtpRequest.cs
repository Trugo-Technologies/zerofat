using System.ComponentModel;
using System.Text.Json.Serialization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Common;
using ZeroFat.Users.Application.Authentication.DTOs;
using ZeroFat.Users.Application.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ZeroFat.Shared.Authorization;
using Mapster;
using ZeroFat.Users.Application.Users;
using ZeroFat.Application.Shared;

namespace ZeroFat.Users.Application.Authentication.Commands;
public class GetTokenByOtpRequest : ICommand<Result<TokenResponse>>
{
    [DefaultValue("admin@innovation.com")]
    public string? Email { get; set; }
    [DefaultValue(null)]
    public string? MobileNumber { get; set; }
    [DefaultValue("985369")]
    public string? OTPPassword { get; set; }
    [DefaultValue(null)]
    public string? FcmToken { get; set; }

    public bool TrustDevice { get; set; }

    [JsonIgnore]
    public TokenRequestHeaders? Headers { get; set; }
}

public class GetTokenByOtpRequestValidator : CustomValidator<GetTokenByOtpRequest>
{
    public GetTokenByOtpRequestValidator() // IStringLocalizer<GetTokenByOtpRequestValidator> localizer
    {
        RuleFor(x => x.Headers).NotNull();
        RuleFor(x => x.Email).NotEmpty().Unless(x => x.MobileNumber.HasValue());
        RuleFor(x => x.MobileNumber).NotEmpty().Unless(x => x.Email.HasValue());
    }
}

public class GetTokenByOtpRequestHandler : ICommandHandler<GetTokenByOtpRequest, Result<TokenResponse>>
{
    private readonly UserManager<ApplicationUser> _applicationUserManager;
    private readonly IRepositoryWithEvents<Device> _deviceRepository;
    private readonly IJwtTokenManagementService _jwtTokenManagementService;
    private readonly IRefreshTokenGenerationService _refreshTokenGenerationService;
    private readonly IStringLocalizer<GetTokenByOtpRequestHandler> _localizer;
    private readonly IClientService _clientService;

    public GetTokenByOtpRequestHandler(
        UserManager<ApplicationUser> applicationUserManager,
        IJwtTokenManagementService jwtTokenManagementService,
        IRefreshTokenGenerationService refreshTokenGenerationService,
        IRepositoryWithEvents<Device> deviceRepository,
        IClientService clientService,
        IStringLocalizer<GetTokenByOtpRequestHandler> localizer)
    {
        _applicationUserManager = applicationUserManager;
        _deviceRepository = deviceRepository;
        _jwtTokenManagementService = jwtTokenManagementService;
        _refreshTokenGenerationService = refreshTokenGenerationService;
        _localizer = localizer;
        _clientService = clientService;
    }

    public async Task<Result<TokenResponse>> Handle(GetTokenByOtpRequest request, CancellationToken cancellationToken)
    {
        if (request.Headers == null || request.Headers.Platform == null || request.Headers.DeviceType == null)
            throw new ForbiddenException("Missing Info");

        ApplicationUser? user = null;

        if (request.MobileNumber.HasValue())
            user = await _applicationUserManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.MobileNumber, cancellationToken: cancellationToken);
        else
            user = await _applicationUserManager.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken: cancellationToken);

        if (user == null)
            throw new NotFoundException("User Not Found");

        var device = await _deviceRepository.FirstOrDefaultAsync(new RequestByDeviceSpec(user.PublicId, request.Headers), cancellationToken);
        if (device == null)
            throw new BadRequestException("Device Not Found");

        var defaulOTPToken = _jwtTokenManagementService.GetDefaulOTPToken();
        if (defaulOTPToken.HasValue() && defaulOTPToken == request.OTPPassword)
        {
            string? token = await _applicationUserManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
            await _applicationUserManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider, token);
        }
        else
        {
            bool isConfirm = await _applicationUserManager.VerifyTwoFactorTokenAsync(user, ZeroFatProviders.FourDigitPhoneProvider, request.OTPPassword);
            if (!isConfirm)
                throw new UnauthorizedException(_localizer["Invalid Credentials"]);
        }

        var roles = await _applicationUserManager.GetRolesAsync(user);

        device.FcmToken = request!.FcmToken;
        device.CurrentLanguage = request.Headers.Language;
        device.LastOTPTime = SystemTime.Now();
        device.TotalOTPConsecutiveCount = 0;
        device.LastLogin = SystemTime.Now();
        device.LastSeen = SystemTime.Now();
        device.IsTrusted = request.TrustDevice;

        var jwtToken = _jwtTokenManagementService.GenerateJwtToken(user, device, roles);
        _refreshTokenGenerationService.GenerateRefreshTokenAsync(device);
        device.Token = jwtToken;

        await _deviceRepository.UpdateAsync(device, withSaveChanges: true, cancellationToken: cancellationToken);
        await _applicationUserManager.ResetAuthenticatorKeyAsync(user);

        var response = new TokenResponse(jwtToken, device.RefreshToken, device.RefreshTokenExpiryTime.Value, user.Adapt<UserDto>());
        if (user.UserType == ZeroFat.Domain.Enums.UserType.Client)
        {
            response.Client = await _clientService.GetClientById(user.PublicId);
            response.DefaultPaymentMethod = await _clientService.GetClientDefaultPaymentMethod(user.PublicId);
            if (response.Client?.ClientSubscriptionId.HasValue == true)
            {
                response.ClientSubscription = await _clientService.GetClientSubscriptionById(response.Client.ClientSubscriptionId.Value);
            }
        }

        return await Result<TokenResponse>.SuccessAsync(data: response);
    }
}
