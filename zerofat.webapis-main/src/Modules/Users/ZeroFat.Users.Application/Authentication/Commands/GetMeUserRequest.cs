using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Users.Application.Authentication.DTOs;
using ZeroFat.Users.Application.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Mapster;
using ZeroFat.Users.Application.Users;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Shared;

namespace ZeroFat.Users.Application.Authentication.Commands;
public class GetMeUserRequest : ICommand<Result<MeResponse>>
{
    
}

public class GetMeUserRequestValidator : CustomValidator<GetMeUserRequest>
{
    public GetMeUserRequestValidator() // IStringLocalizer<GetMeUserRequestValidator> localizer
    {
    }
}

public class GetMeUserRequestHandler : ICommandHandler<GetMeUserRequest, Result<MeResponse>>
{
    private readonly UserManager<ApplicationUser> _applicationUserManager;
    private readonly IClientService _clientService;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer<GetMeUserRequestHandler> _localizer;

    public GetMeUserRequestHandler(
        UserManager<ApplicationUser> applicationUserManager,
        IJwtTokenManagementService jwtTokenManagementService,
        ICurrentUser currentUser,
        IClientService clientService,
        IStringLocalizer<GetMeUserRequestHandler> localizer)
    {
        _applicationUserManager = applicationUserManager;
        _localizer = localizer;
        _clientService = clientService;
        _currentUser = currentUser;
    }

    public async Task<Result<MeResponse>> Handle(GetMeUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _applicationUserManager.Users.Where(x => x.PublicId == _currentUser.GetUserId()).ProjectToType<UserDto>().FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (user == null)
            throw new NotFoundException("User Not Found");

        var response = new MeResponse(user);
        if (user.UserType == ZeroFat.Domain.Enums.UserType.Client)
        {
            response.Client = await _clientService.GetClientById(user.PublicId);
            response.DefaultPaymentMethod = await _clientService.GetClientDefaultPaymentMethod(user.PublicId);
            if (response.Client?.ClientSubscriptionId.HasValue == true)
            {
                response.ClientSubscription = await _clientService.GetClientSubscriptionById(response.Client.ClientSubscriptionId.Value);
            }
        }

        return await Result<MeResponse>.SuccessAsync(data: response);
    }
}
