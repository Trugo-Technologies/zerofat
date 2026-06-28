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
using Mapster;
using ZeroFat.Users.Application.Users;
using ZeroFat.Application.Shared;

namespace ZeroFat.Users.Application.Authentication.Commands;
public class GetTokenWith2FARequest : ICommand<Result<TokenResponse>>
{
    [DefaultValue("admin@innovation.com")]
    public string? Email { get; set; }
    [DefaultValue(null)]
    public string? MobileNumber { get; set; }
    public string? TwoFactorToken { get; set; }
    public string? FcmToken { get; set; }
    public bool TrustDevice { get; set; }

    [JsonIgnore]
    public TokenRequestHeaders? Headers { get; set; }
}

public class GetTokenWith2FARequestValidator : CustomValidator<GetTokenWith2FARequest>
{
    public GetTokenWith2FARequestValidator() // IStringLocalizer<GetTokenWith2FARequestValidator> localizer
    {
        RuleFor(x => x.Headers).NotNull();
        RuleFor(x => x.Email).NotEmpty().Unless(x => x.MobileNumber.HasValue());
        RuleFor(x => x.MobileNumber).NotEmpty().Unless(x => x.Email.HasValue());
    }
}

public class GetTokenWith2FARequestHandler : ICommandHandler<GetTokenWith2FARequest, Result<TokenResponse>>
{
    private readonly UserManager<ApplicationUser> _applicationUserManager;
    private readonly IRepositoryWithEvents<Device> _deviceRepository;
    private readonly IJwtTokenManagementService _jwtTokenManagementService;
    private readonly IRefreshTokenGenerationService _refreshTokenGenerationService;
    private readonly IStringLocalizer<GetTokenWith2FARequestHandler> _localizer;
    private readonly IClientService _clientService;

    public GetTokenWith2FARequestHandler(
        UserManager<ApplicationUser> applicationUserManager,
        IJwtTokenManagementService jwtTokenManagementService,
        IRefreshTokenGenerationService refreshTokenGenerationService,
        IRepositoryWithEvents<Device> deviceRepository,
        IClientService clientService,
        IStringLocalizer<GetTokenWith2FARequestHandler> localizer)
    {
        _applicationUserManager = applicationUserManager;
        _deviceRepository = deviceRepository;
        _jwtTokenManagementService = jwtTokenManagementService;
        _refreshTokenGenerationService = refreshTokenGenerationService;
        _localizer = localizer;
        _clientService = clientService;
    }

    public async Task<Result<TokenResponse>> Handle(GetTokenWith2FARequest request, CancellationToken cancellationToken)
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

        if (user.UserType == ZeroFat.Domain.Enums.UserType.Client)
            await _clientService.EnsureClientCanLoginAsync(user.PublicId);

        var device = await _deviceRepository.FirstOrDefaultAsync(new RequestByDeviceSpec(user.PublicId, request.Headers), cancellationToken);
        if (device == null)
            throw new BadRequestException("Device Not Found");

        bool isConfirm = await _applicationUserManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider, request.TwoFactorToken);
        if (!isConfirm)
            throw new UnauthorizedException(_localizer["Invalid Credentials"]);

        device.FcmToken = request!.FcmToken;
        device.IsTrusted = request.TrustDevice;
        device.CurrentLanguage = request.Headers.Language;
        device.LastOTPTime = SystemTime.Now();
        device.TotalOTPConsecutiveCount = 0;
        device.LastLogin = SystemTime.Now();
        device.LastSeen = SystemTime.Now();

        var roles = await _applicationUserManager.GetRolesAsync(user);
        var jwtToken = _jwtTokenManagementService.GenerateJwtToken(user, device, roles);
        _refreshTokenGenerationService.GenerateRefreshTokenAsync(device);
        device.Token = jwtToken;

        await _deviceRepository.UpdateAsync(device, cancellationToken);
        await _applicationUserManager.ResetAuthenticatorKeyAsync(user);

        var response = new TokenResponse(jwtToken, device.RefreshToken, device.RefreshTokenExpiryTime.Value, user.Adapt<UserDto>());

        if (user.UserType == ZeroFat.Domain.Enums.UserType.Client)
        {
            response.Client = await _clientService.GetClientById(user.PublicId);
            if (response.Client?.ClientSubscriptionId.HasValue == true)
            {
                response.ClientSubscription = await _clientService.GetClientSubscriptionById(response.Client.ClientSubscriptionId.Value);
            }
        }

        return await Result<TokenResponse>.SuccessAsync(data: response);
    }
}
