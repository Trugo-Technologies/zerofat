using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Application.Shared;
using ZeroFat.Domain.Common;
using ZeroFat.Domain.Enums;
using ZeroFat.Shared.Authorization;
using ZeroFat.Users.Application.Common.Extensions;

namespace ZeroFat.Users.Application.Authentication.Commands;
public class GetClientOtpTokenRequest : ICommand<Result<string>>
{
    public string? MobileNumber { get; set; }
    public string? AppSignatureId { get; set; }
    public string? FcmToken { get; set; }

    [JsonIgnore]
    public TokenRequestHeaders? Headers { get; set; }
}


public class GetClientOtpTokenRequestValidator : CustomValidator<GetClientOtpTokenRequest>
{
    public GetClientOtpTokenRequestValidator() // IStringLocalizer<GetClientOtpTokenRequestValidator> localizer
    {
        RuleFor(x => x.Headers).NotNull();
        RuleFor(x => x.MobileNumber).NotEmpty();
    }
}

public class GetClientOtpTokenRequestHandler : ICommandHandler<GetClientOtpTokenRequest, Result<string>>
{
    private readonly UserManager<ApplicationUser> _applicationUserManager;
    private readonly IRepository<Device> _deviceRepository;
    private readonly IStringLocalizer<GetClientOtpTokenRequestHandler> _localizer;
    private readonly IClientService _clientService;

    private readonly IJobService _jobService;

    public GetClientOtpTokenRequestHandler(
        UserManager<ApplicationUser> applicationUserManager,
        IStringLocalizer<GetClientOtpTokenRequestHandler> localizer,
        IJobService jobService,
        IClientService clientService,
        IRepository<Device> deviceRepository)
    {
        _applicationUserManager = applicationUserManager;
        _deviceRepository = deviceRepository;
        _localizer = localizer;
        _jobService = jobService;
        _clientService = clientService;
    }

    public async Task<Result<string>> Handle(GetClientOtpTokenRequest request, CancellationToken cancellationToken)
    {
        if (request.Headers == null || request.Headers.Platform == null || request.Headers.DeviceType == null)
            throw new ForbiddenException("Missing Info");

        ApplicationUser? user = await _applicationUserManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.MobileNumber, cancellationToken: cancellationToken);

        if (user == null)
        {
            user = new ApplicationUser()
            {
                UserName = request.MobileNumber,
                PhoneNumber = request.MobileNumber,
                UserType = UserType.Client,
                LoginMechanism = LoginMechanism.OTP,
                IsActive = true,
            };

            var result = await _applicationUserManager.CreateAsync(user);
            if (!result.Succeeded)
                throw new InternalServerException("Validation Errors Occurred.", result.GetErrors(_localizer));

            await _applicationUserManager.AddToRoleAsync(user, ZeroFatRoles.Client);
        }
        else
        {
            await _clientService.EnsureClientCanLoginAsync(user.PublicId);
        }

        string? token = await _applicationUserManager.GenerateTwoFactorTokenAsync(user, ZeroFatProviders.FourDigitPhoneProvider);
        var device = await _deviceRepository.FirstOrDefaultAsync(new RequestByDeviceSpec(user.PublicId, request.Headers), cancellationToken);
        if (device == null)
        {
            device = new Device
            {
                UserPublicId = user.PublicId,
                BaseDeviceId = request.Headers.BaseDeviceId,
                DeviceModel = request.Headers.DeviceModel,
                DeviceType = request.Headers.DeviceType.Value,
                DeviceOs = request.Headers.DeviceOs,
                Version = request.Headers.Version,
                Platform = request.Headers.Platform,

                TotalOTPConsecutiveCount = 1,
                TotalOTPCount = 1,
                LastOTPTime = SystemTime.Now(),
                FcmToken = request.FcmToken,
                IPAddressOnCreated = request.Headers.IPAddress,
                LastKnownIPAddress = request.Headers.IPAddress,
                LastVerificationCode = token,
            };

            await _deviceRepository.AddAsync(device, cancellationToken);
        }
        else
        {
            if (device.LastOTPTime.HasValue)
            {
                var time = SystemTime.Now() - device.LastOTPTime.Value;

                if (device.TotalOTPConsecutiveCount >= 2 && time.TotalSeconds <= (device.TotalOTPConsecutiveCount - 1) * 2 * 60)
                    throw new OtpRequestLimitExceededException("Invalid OTP", errors: new List<string>() { ((device.TotalOTPConsecutiveCount - 1) * 2 * 60 - (int)time.TotalSeconds).ToString() });

                if (device.TotalOTPConsecutiveCount >= 7 && time.TotalSeconds <= 60 * 2 * 60)
                    throw new OtpRequestLimitExceededException("Invalid OTP", errors: new List<string>() { (2 * 60 * 60 - (int)time.TotalSeconds).ToString() });

                if (time.TotalSeconds >= 60 * 60 * 2)
                    device.TotalOTPConsecutiveCount = 0;
            }

            device.DeviceModel = request.Headers.DeviceModel;
            device.DeviceOs = request.Headers.DeviceOs;
            device.TotalOTPCount++;
            device.TotalOTPConsecutiveCount++;
            device.LastVerificationCode = token;
            device.LastOTPTime = SystemTime.Now();

            await _deviceRepository.UpdateAsync(device, cancellationToken);
        }

        _jobService.Enqueue<ISMSService>(x => x.SendAsync(user.PhoneNumber!, $"Your ZEROFAT account's OTP code is: {token}", true, cancellationToken));

        return await Result<string>.SuccessAsync();
    }
}


