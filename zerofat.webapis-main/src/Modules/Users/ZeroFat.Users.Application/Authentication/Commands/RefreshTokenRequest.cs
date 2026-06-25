using System.Text.Json.Serialization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Users.Application.Authentication.DTOs;
using ZeroFat.Users.Application.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mapster;
using ZeroFat.Users.Application.Users;
using ZeroFat.Application.Common.Specification;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Shared;

namespace ZeroFat.Users.Application.Authentication.Commands;
public class RefreshTokenRequest : ICommand<Result<TokenResponse>>
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }

    [JsonIgnore]
    public TokenRequestHeaders? Headers { get; set; }
}

public class RefreshTokenRequestValidator : CustomValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Headers).NotNull();

        RuleFor(x => x.Token)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.RefreshToken)
            .NotNull()
            .NotEmpty();
    }
}

public class RefreshTokenRequestHandler : ICommandHandler<RefreshTokenRequest, Result<TokenResponse>>
{
    private readonly UserManager<ApplicationUser> _applicationUserManager;
    private readonly IRepositoryWithEvents<Device> _deviceRepository;
    private readonly IClientService _clientService;
    private readonly IJwtTokenManagementService _jwtTokenManagementService;
    private readonly IRefreshTokenGenerationService _refreshTokenGenerationService;
    private readonly IStringLocalizer<RefreshTokenRequestHandler> _localizer;

    public RefreshTokenRequestHandler(
        UserManager<ApplicationUser> applicationUserManager,
        IJwtTokenManagementService jwtTokenManagementService,
        IRefreshTokenGenerationService refreshTokenGenerationService,
        IClientService clientService,
        IRepositoryWithEvents<Device> deviceRepository,
        IStringLocalizer<RefreshTokenRequestHandler> localizer)
    {
        _applicationUserManager = applicationUserManager;
        _deviceRepository = deviceRepository;
        _jwtTokenManagementService = jwtTokenManagementService;
        _refreshTokenGenerationService = refreshTokenGenerationService;
        _clientService = clientService;
        _localizer = localizer;
    }

    public async Task<Result<TokenResponse>> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (request.Headers == null || request.Headers.Platform == null || request.Headers.DeviceType == null)
            throw new ForbiddenException("Missing Info");

        var userPublicId = _jwtTokenManagementService.GetUserIdPrincipalFromExpiredToken(request.Token);
        if (!userPublicId.HasValue())
            throw new BadRequestException("Invalid token");
        var guidId = Guid.Parse(userPublicId);
        ApplicationUser? user = await _applicationUserManager.Users.FirstOrDefaultAsync(x => x.PublicId == guidId, cancellationToken);

        if (user == null)
            throw new NotFoundException(_localizer["User Not Found"]);

        var device = await _deviceRepository.FirstOrDefaultAsync(new ExpressionSpecification<Device>(p => p.UserPublicId == user.PublicId && p.BaseDeviceId == request.Headers.BaseDeviceId && p.Platform == request.Headers.Platform), cancellationToken);
        if (device is null)
            throw new BadRequestException(_localizer["Device not found"]);

        if (device.RefreshToken != request.RefreshToken || device.RefreshTokenExpiryTime < DateTime.UtcNow)
            throw new UnauthorizedException(_localizer["Invalid Refresh Token."]);

        var roles = await _applicationUserManager.GetRolesAsync(user);

        var token = _jwtTokenManagementService.GenerateJwtToken(user, device, roles);
        _refreshTokenGenerationService.GenerateRefreshTokenAsync(device);

        device.Token = token;

        await _deviceRepository.UpdateAsync(device, withSaveChanges: true, cancellationToken: cancellationToken);
        await _applicationUserManager.ResetAuthenticatorKeyAsync(user);

        var response = new TokenResponse(token, device.RefreshToken, device.RefreshTokenExpiryTime.Value, user.Adapt<UserDto>());
        if (user.UserType == ZeroFat.Domain.Enums.UserType.Client)
        {
            response.Client = await _clientService.GetClientById(user.PublicId);
            response.DefaultPaymentMethod = await _clientService.GetClientDefaultPaymentMethod(user.PublicId);
            if (response.Client?.ClientSubscriptionId.HasValue == true)
            {
                response.ClientSubscription = await _clientService.GetClientSubscriptionById(response.Client.ClientSubscriptionId.Value);
            }
        }

        return await Result<TokenResponse>.SuccessAsync(response);
    }


}
