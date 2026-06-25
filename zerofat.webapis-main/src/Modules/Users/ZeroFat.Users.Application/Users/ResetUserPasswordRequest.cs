using System.ComponentModel;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Users.Application.Common.Errors;
using ZeroFat.Users.Application.Common.Extensions.Validations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ZeroFat.Users.Application.Users;
public sealed class ResetUserPasswordRequest : ICommand<Result>
{
    public Guid? UserId { get; set; }
    [DefaultValue("P@ssw0rd")]
    public string? NewPassword { get; set; }
}

public class ResetUserPasswordRequestValidator : CustomValidator<ResetUserPasswordRequest>
{
    public ResetUserPasswordRequestValidator() // IStringLocalizer<ResetUserPasswordRequestValidator> localizer
    {
        RuleFor(p => p.UserId).NotEmpty();
        RuleFor(p => p.NewPassword)
           .NotEmpty()
           // Minimum eight and maximum 25 characters, at least one uppercase letter, one lowercase letter, one number and one special character
           .MustBeCorrectPasswordFormat();
    }
}


public class ResetUserPasswordRequestHandler : ICommandHandler<ResetUserPasswordRequest, Result>
{
    private readonly UserManager<ApplicationUser> _applicationUserManager;
    private readonly IStringLocalizer<ResetUserPasswordRequestHandler> _localizer;

    public ResetUserPasswordRequestHandler(
        UserManager<ApplicationUser> applicationUserManager,
        IStringLocalizer<ResetUserPasswordRequestHandler> localizer)
    {
        _applicationUserManager = applicationUserManager;
        _localizer = localizer;
    }

    public async Task<Result> Handle(ResetUserPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _applicationUserManager.Users.Where(u => u.PublicId == request.UserId.Value).FirstOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(_localizer["User not found."]);
        }

        var token = await _applicationUserManager.GeneratePasswordResetTokenAsync(user);
        var result = await _applicationUserManager.ResetPasswordAsync(user, token, request.NewPassword!);
        if (result.Succeeded)
        {
            return (Result)await Result.SuccessAsync();
        }

        var tokenExpiredError = result.Errors.SingleOrDefault(c => c.Code == IndentityErrorsCodes.TokenExpired);
        if (tokenExpiredError != null)
        {
            throw new BadRequestException(_localizer["Token Expired"]);
        }

        var passwordRepeatedError = result.Errors.SingleOrDefault(c => c.Code == IndentityErrorsCodes.PasswordRepeated);
        if (passwordRepeatedError != null)
        {
            throw new BadRequestException(_localizer["Password Reapted"]);
        }

        throw new BadRequestException(_localizer["Error during reset password for user: {0}. Message: {1}", user, result.Errors]);
    }
}
