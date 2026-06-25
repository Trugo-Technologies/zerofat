using Ardalis.Specification;
using System.Text.Json.Serialization;
using FluentValidation;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.Users.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Domain.Common;
using System.ComponentModel;

namespace ZeroFat.Users.Application.Authentication.Commands;
public class GetOtpTokenRequest : ICommand<Result<string>>
{
    public string? MobileNumber { get; set; }
    [DefaultValue("admin@innovation.com")]
    public string? Email { get; set; }
    public string? AppSignatureId { get; set; }
    public string? FcmToken { get; set; }

    [JsonIgnore]
    public TokenRequestHeaders? Headers { get; set; }
}

public class TokenRequestHeaders
{
    public string? BaseDeviceId { get; set; } = default!;
    public string? DeviceModel { get; set; }
    public string? DeviceOs { get; set; }
    public string? Version { get; set; }
    public DeviceType? DeviceType { get; set; }
    public string? Platform { get; set; }
    public string? Language { get; set; }
    public string? IPAddress { get; set; }
}

public class GetOtpTokenRequestValidator : CustomValidator<GetOtpTokenRequest>
{
    public GetOtpTokenRequestValidator() // IStringLocalizer<GetOtpTokenRequestValidator> localizer
    {
        RuleFor(x => x.Headers).NotNull();
        RuleFor(x => x.Email).NotEmpty().Unless(x => x.MobileNumber.HasValue());
        RuleFor(x => x.MobileNumber).NotEmpty().Unless(x => x.Email.HasValue());
    }
}

public class RequestByDeviceSpec : Specification<Device>
{
    public RequestByDeviceSpec(Guid userPublicId, TokenRequestHeaders headers) =>
        Query.Where(p => p.UserPublicId == userPublicId && p.BaseDeviceId == headers.BaseDeviceId && p.Platform == headers.Platform);
}

public class DeviceByRefreshTokenSpec : Specification<Device>
{
    public DeviceByRefreshTokenSpec(Guid userPublicId, string refreshtoken) =>
        Query.Where(p => p.UserPublicId == userPublicId && p.RefreshToken == refreshtoken);
}


public class GetOtpTokenRequestHandler : ICommandHandler<GetOtpTokenRequest, Result<string>>
{
    private readonly UserManager<ApplicationUser> _applicationUserManager;
    private readonly IRepository<Device> _deviceRepository;
    public GetOtpTokenRequestHandler(
        UserManager<ApplicationUser> applicationUserManager,
        IRepository<Device> deviceRepository)
    {
        _applicationUserManager = applicationUserManager;
        _deviceRepository = deviceRepository;
    }

    public async Task<Result<string>> Handle(GetOtpTokenRequest request, CancellationToken cancellationToken)
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

        string? token = await _applicationUserManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
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

        // await _applicationUserManager.UpdateAsync(user);
        // _jobService.Enqueue(() => _sMSService.SendAsync(user.PhoneNumber, message, isArabic));


        return await Result<string>.SuccessAsync(data: token);
    }
}
