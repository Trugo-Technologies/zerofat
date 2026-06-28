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
using Mapster;
using ZeroFat.Users.Application.Users;
using ZeroFat.Application.Shared;

namespace ZeroFat.Users.Application.Authentication.Commands;
public class GetTokenRequest : ICommand<Result<TokenResponse>>
{
    [DefaultValue("admin@innovation.com")]
    public string? Email { get; set; }
    [DefaultValue(null)]
    public string? MobileNumber { get; set; }
    [DefaultValue("P@ssw0rd@2024")]
    public string? Password { get; set; }
    [DefaultValue(null)]
    public string? FcmToken { get; set; }

    public bool TrustDevice { get; set; }

    [JsonIgnore]
    public TokenRequestHeaders? Headers { get; set; }
}

public class GetTokenRequestValidator : CustomValidator<GetTokenRequest>
{
    public GetTokenRequestValidator() // IStringLocalizer<GetTokenRequestValidator> localizer
    {
        RuleFor(x => x.Headers).NotNull();
        RuleFor(x => x.Email).NotEmpty().Unless(x => x.MobileNumber.HasValue());
        RuleFor(x => x.MobileNumber).NotEmpty().Unless(x => x.Email.HasValue());
    }
}

public class GetTokenRequestHandler : ICommandHandler<GetTokenRequest, Result<TokenResponse>>
{
    private readonly UserManager<ApplicationUser> _applicationUserManager;
    private readonly IRepositoryWithEvents<Device> _deviceRepository;
    private readonly IClientService _clientService;
    private readonly IJwtTokenManagementService _jwtTokenManagementService;
    private readonly IRefreshTokenGenerationService _refreshTokenGenerationService;

    public GetTokenRequestHandler(
        UserManager<ApplicationUser> applicationUserManager,
        IJwtTokenManagementService jwtTokenManagementService,
        IRefreshTokenGenerationService refreshTokenGenerationService,
        IClientService clientService,
        IRepositoryWithEvents<Device> deviceRepository)
    {
        _applicationUserManager = applicationUserManager;
        _deviceRepository = deviceRepository;
        _jwtTokenManagementService = jwtTokenManagementService;
        _refreshTokenGenerationService = refreshTokenGenerationService;
        _clientService = clientService;
    }

    public async Task<Result<TokenResponse>> Handle(GetTokenRequest request, CancellationToken cancellationToken)
    {
        if (request.Headers == null || request.Headers.Platform == null || request.Headers.DeviceType == null)
            throw new ForbiddenException("Missing Info");

        ApplicationUser? user = null;

        if (request.MobileNumber.HasValue())
            user = await _applicationUserManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.PhoneNumber == request.MobileNumber, cancellationToken: cancellationToken);
        else
            user = await _applicationUserManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken: cancellationToken);

        if (user == null)
            throw new NotFoundException("User Not Found");

        bool passwordValid = await _applicationUserManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            throw new UnauthorizedException("Identity Invalid Credentials");

        if (user.UserType == ZeroFat.Domain.Enums.UserType.Client)
            await _clientService.EnsureClientCanLoginAsync(user.PublicId);

        var roles = await _applicationUserManager.GetRolesAsync(user);
        var device = await _deviceRepository.FirstOrDefaultAsync(new RequestByDeviceSpec(user.PublicId, request.Headers), cancellationToken);
        bool isnew = false;
        bool isTrusted = false;
        if (device == null)
        {
            isnew = true;
            device = new Device
            {
                UserPublicId = user.PublicId,
                BaseDeviceId = request.Headers.BaseDeviceId,
                DeviceModel = request.Headers.DeviceModel,
                DeviceType = request.Headers.DeviceType.Value,
                DeviceOs = request.Headers.DeviceOs,
                Version = request.Headers.Version,
                Platform = request.Headers.Platform,
                CurrentLanguage = request.Headers.Language,
                TotalOTPConsecutiveCount = 0,
                TotalOTPCount = 0,
                LastOTPTime = null,
                FcmToken = request.FcmToken,
                IPAddressOnCreated = request.Headers.IPAddress,
                LastKnownIPAddress = request.Headers.IPAddress,
                LastLogin = SystemTime.Now(),
                LastSeen = SystemTime.Now(),
                LastVerificationCode = null,
                Token = null,
                RefreshToken = null,
                RefreshTokenExpiryTime = null
            };

            if (!user.TwoFactorEnabled)
                device.IsTrusted = request.TrustDevice;
        }
        else
        {
            isTrusted = device.IsTrusted;
            device.DeviceModel = request.Headers.DeviceModel;
            device.DeviceOs = request.Headers.DeviceOs;
            device.FcmToken = request.FcmToken;
            device.CurrentLanguage = request.Headers.Language;
            device.LastLogin = SystemTime.Now();
            device.LastSeen = SystemTime.Now();

            if (!user.TwoFactorEnabled)
                device.IsTrusted = request.TrustDevice;
        }

        if (!isTrusted && user.TwoFactorEnabled)
        {
            if (device.LastOTPTime.HasValue)
            {
                var time = SystemTime.Now() - device.LastOTPTime.Value;

                if (device.TotalOTPConsecutiveCount >= 2 && time.TotalSeconds <= (device.TotalOTPConsecutiveCount - 1) * 2 * 60)
                    throw new ForbiddenException("Invalid OTP", errors: new List<string>() { ((device.TotalOTPConsecutiveCount - 1) * 2 * 60 - (int)time.TotalSeconds).ToString() });

                if (device.TotalOTPConsecutiveCount >= 7 && time.TotalSeconds <= 60 * 2 * 60)
                    throw new ForbiddenException("Invalid OTP", errors: new List<string>() { (2 * 60 * 60 - (int)time.TotalSeconds).ToString() });

                if (time.TotalSeconds >= 60 * 60 * 2)
                    device.TotalOTPConsecutiveCount = 0;
            }

            string? token = await _applicationUserManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);

            device.TotalOTPCount++;
            device.TotalOTPConsecutiveCount++;
            device.LastVerificationCode = token;
            device.LastOTPTime = SystemTime.Now();

            if (isnew)
                await _deviceRepository.AddAsync(device, withSaveChanges: true, cancellationToken: cancellationToken);
            else
                await _deviceRepository.UpdateAsync(device, withSaveChanges: true, cancellationToken: cancellationToken);

            return await Result<TokenResponse>.SuccessAsync(data: new TokenResponse(true, token));
        }
        else
        {
            if (isnew)
                await _deviceRepository.AddAsync(device, withSaveChanges: true, cancellationToken: cancellationToken);
            else
                await _deviceRepository.UpdateAsync(device, withSaveChanges: true, cancellationToken: cancellationToken);

            var jwtToken = _jwtTokenManagementService.GenerateJwtToken(user, device, roles);
            _refreshTokenGenerationService.GenerateRefreshTokenAsync(device);
            device.Token = jwtToken;
            var response = new TokenResponse(jwtToken, device.RefreshToken, device.RefreshTokenExpiryTime.Value, user.Adapt<UserDto>());

            if(user.UserType == ZeroFat.Domain.Enums.UserType.Client)
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
}
