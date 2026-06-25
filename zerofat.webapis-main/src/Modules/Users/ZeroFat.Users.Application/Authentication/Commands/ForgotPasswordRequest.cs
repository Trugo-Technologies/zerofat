using FluentValidation;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Users.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ZeroFat.Users.Application.Authentication.Commands;
public sealed class ForgotPasswordRequest : ICommand<Result>
{
    public string? Email { get; set; }
}

public class ForgotPasswordRequestValidator : CustomValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator() // IStringLocalizer<ForgotPasswordRequestValidator> localizer
    {
        RuleFor(f => f.Email)
            .NotEmpty();
    }
}


public class ForgotPasswordRequestHandler(
    UserManager<ApplicationUser> applicationUserManager) : ICommandHandler<ForgotPasswordRequest, Result>
{
    private readonly UserManager<ApplicationUser> _applicationUserManager = applicationUserManager;

    public async Task<Result> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _applicationUserManager.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user == null)
        {
            return (Result)await Result.FailAsync();
        }

        if (!user.EmailConfirmed)
        {
            throw new InvalidOperationException("User email is not confirmed");
        }

        var resetPasswordToken = await _applicationUserManager.GeneratePasswordResetTokenAsync(user);

        return (Result)await Result.SuccessAsync();
    }
}
