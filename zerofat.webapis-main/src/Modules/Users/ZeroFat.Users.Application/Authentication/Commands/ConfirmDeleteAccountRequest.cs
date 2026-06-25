using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Application.Shared;
using ZeroFat.Shared.Authorization;

namespace ZeroFat.Users.Application.Authentication.Commands;
public sealed class ConfirmDeleteAccountRequest : ICommand<Result>
{
    [DefaultValue("123456")]
    public string? Token { get; set; }
}

public class ConfirmDeleteAccountRequestValidator : CustomValidator<ConfirmDeleteAccountRequest>
{
    public ConfirmDeleteAccountRequestValidator()
    {
        RuleFor(p => p.Token).NotEmpty();
    }
}


public class ConfirmDeleteAccountRequestHandler : ICommandHandler<ConfirmDeleteAccountRequest, Result>
{
    private readonly UserManager<ApplicationUser> _applicationUserManager;
    private readonly ICurrentUser _currentUser;
    private readonly IRepositoryWithEvents<ApplicationUser> _repository;
    private readonly IClientService _clientService;
    public ConfirmDeleteAccountRequestHandler(
        UserManager<ApplicationUser> applicationUserManager,
        IClientService clientService,
        IRepositoryWithEvents<ApplicationUser> repository,
        ICurrentUser currentUser)
    {
        _applicationUserManager = applicationUserManager;
        _currentUser = currentUser;
        _clientService = clientService;
        _repository = repository;
    }

    public async Task<Result> Handle(ConfirmDeleteAccountRequest request, CancellationToken cancellationToken)
    {
        var user = await _applicationUserManager.Users.Where(u => u.PublicId == _currentUser.GetUserId()).FirstOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        var isValidOtp = await _applicationUserManager.VerifyTwoFactorTokenAsync(user, ZeroFatProviders.FourDigitPhoneProvider, request.Token!);
        if (!isValidOtp)
        {
            throw new BadRequestException("Invalid or expired token.");
        }

        // user.DeletedOn = SystemTime.Now();
        // user.DeletedByName = "Account Owner";
        // user.DeletedBy = _currentUser.GetUserId();
        // user.UserName += "_Deleted";
        // user.PhoneNumber += "_Deleted";
        // if (user.Email.HasValue())
        // {
        //     user.Email += "_Deleted";
        // }
        // 
        // var result = await _applicationUserManager.UpdateAsync(user);
        await _repository.DeleteAsync(user, withTracking: false, cancellationToken: cancellationToken);
        await _clientService.DeactivateClientAsync(user.PublicId);
        return (Result)await Result.SuccessAsync();

        // if (result.Succeeded)
        // {
        //     return (Result)await Result.SuccessAsync();
        // }
        // 
        // throw new BadRequestException($"Error during account deletion for user: {user}. Message: {string.Join(",", result.Errors)}");
    }
}
