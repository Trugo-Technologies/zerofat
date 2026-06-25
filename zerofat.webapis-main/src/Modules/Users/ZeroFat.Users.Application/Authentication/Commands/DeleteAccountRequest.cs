using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Shared.Authorization;

namespace ZeroFat.Users.Application.Authentication.Commands;
public sealed class DeleteAccountRequest : ICommand<Result<string>>
{
}

public class DeleteAccountRequestValidator : CustomValidator<DeleteAccountRequest>
{
    public DeleteAccountRequestValidator()
    {
       
    }
}


public class DeleteAccountRequestHandler(
    UserManager<ApplicationUser> applicationUserManager, ICurrentUser currentUser, IJobService jobService) : ICommandHandler<DeleteAccountRequest, Result<string>>
{
    public async Task<Result<string>> Handle(DeleteAccountRequest request, CancellationToken cancellationToken)
    {
        var user = await applicationUserManager.Users.SingleOrDefaultAsync(u => u.PublicId == currentUser.GetUserId(), cancellationToken);
        if (user == null)
        {
            // We don't want to reveal if a user exists or not for security reasons
            return await Result<string>.SuccessAsync();
        }

        // if (!user.EmailConfirmed)
        // {
        //     // Depending on business logic, you might want to handle this differently.
        //     // For this example, we'll prevent unconfirmed emails from proceeding.
        //     throw new InvalidOperationException("User email is not confirmed");
        // }

        string? deleteToken = await applicationUserManager.GenerateTwoFactorTokenAsync(user, ZeroFatProviders.FourDigitPhoneProvider);

        // In a real application, you would send this token to the user's email.
        // For example:

        jobService.Enqueue<ISMSService>(x => x.SendAsync(user.PhoneNumber!, $"Your One-Time Password (OTP) to permanently delete your ZEROFAT account is: {deleteToken}", true, cancellationToken));

        // await _emailSender.SendEmailAsync(user.Email, "Confirm your account deletion", $"Use this token to delete your account: {deleteToken}");

        return await Result<string>.SuccessAsync(); // data: deleteToken
    }
}
