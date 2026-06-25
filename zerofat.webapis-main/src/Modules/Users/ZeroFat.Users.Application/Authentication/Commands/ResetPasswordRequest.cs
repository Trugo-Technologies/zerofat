using System.ComponentModel;
using FluentValidation;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Users.Application.Common.Errors;
using ZeroFat.Users.Application.Common.Extensions.Validations;
using ZeroFat.Users.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ZeroFat.Users.Application.Authentication.Commands;
public sealed class ResetPasswordRequest : ICommand<Result>
{
    public Guid? UserId { get; set; }
    [DefaultValue("P@ssw0rd")]
    public string? NewPassword { get; set; }
    [DefaultValue("123456")]
    public string? Token { get; set; }
}

public class ResetPasswordRequestValidator : CustomValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator() // IStringLocalizer<ResetPasswordRequestValidator> localizer
    {
        RuleFor(p => p.Token).NotEmpty();
        RuleFor(p => p.UserId).NotEmpty();
        RuleFor(p => p.NewPassword)
           .NotEmpty()
           // Minimum eight and maximum 25 characters, at least one uppercase letter, one lowercase letter, one number and one special character
           .MustBeCorrectPasswordFormat();
    }
}


public class ResetPasswordRequestHandler : ICommandHandler<ResetPasswordRequest, Result>
{
    private readonly UserManager<ApplicationUser> _applicationUserManager;
    public ResetPasswordRequestHandler(
        UserManager<ApplicationUser> applicationUserManager)
    {
        _applicationUserManager = applicationUserManager;
    }

    public async Task<Result> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _applicationUserManager.Users.Where(u => u.PublicId == request.UserId.Value).FirstOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        var result = await _applicationUserManager.ResetPasswordAsync(user, request.Token!, request.NewPassword!);
        if (result.Succeeded)
        {
            return (Result)await Result.SuccessAsync();
        }

        var tokenExpiredError = result.Errors.SingleOrDefault(c => c.Code == IndentityErrorsCodes.TokenExpired);
        if (tokenExpiredError != null)
        {
            throw new BadRequestException("Token Expired");
        }

        var passwordRepeatedError = result.Errors.SingleOrDefault(c => c.Code == IndentityErrorsCodes.PasswordRepeated);
        if (passwordRepeatedError != null)
        {
            throw new BadRequestException("Password Reapted");
        }

        throw new BadRequestException($"Error during reset password for user: {user}. Message: {result.Errors}");
    }
}
