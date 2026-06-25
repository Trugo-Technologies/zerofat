using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Shared;
using ZeroFat.Shared.Authorization;

namespace ZeroFat.Users.Application.Users;
public class UpdateUserPhoneOrEmailRequest : ICommand<Result<string>>
{
    public string? Code { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

public class UpdateUserPhoneOrEmailRequestValidator : AbstractValidator<UpdateUserPhoneOrEmailRequest>
{
    public UpdateUserPhoneOrEmailRequestValidator(IReadRepository<ApplicationUser> repository, IStringLocalizer<UpdateUserPhoneOrEmailRequestValidator> localaizer)
    {
        When(x => x.Email.HasValue(), () =>
        {
            RuleFor(u => u.Email)
               .Cascade(CascadeMode.Stop)
               .NotEmpty()
               .MustAsync(async (email, _) => !await repository.AnyAsync(new ExpressionSpecification<ApplicationUser>(x => x.Email == email), _))
                    .WithMessage(localaizer["Email already exists"]);
        });

        When(x => x.PhoneNumber.HasValue(), () =>
        {
            RuleFor(u => u.PhoneNumber)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (mobile, _) => !await repository.AnyAsync(new ExpressionSpecification<ApplicationUser>(x => x.PhoneNumber == mobile), _))
                .WithMessage(localaizer["Mobile number already exists"]);
        });

    }
}

public class UpdateUserPhoneOrEmailRequestHandler(
    UserManager<ApplicationUser> userManger, 
    IStringLocalizer<UpdateUserPhoneOrEmailRequestHandler> localizer,
    IClientService clientService,
    ICurrentUser currentUser) : IRequestHandler<UpdateUserPhoneOrEmailRequest, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManger = userManger;
    private readonly IStringLocalizer<UpdateUserPhoneOrEmailRequestHandler> _localizer = localizer;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IClientService _clientService = clientService;

    public async Task<Result<string>> Handle(UpdateUserPhoneOrEmailRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManger.Users.FirstOrDefaultAsync(x => x.PublicId == _currentUser.GetUserId(), cancellationToken);

        if (user is null)
            throw new NotFoundException(_localizer["User not found"]);

        if (request.Code.HasValue())
            await Update(user, request.Code, request.Email, request.PhoneNumber);
        else
        {
            var token = await SendCode(user, cancellationToken);
            return await Result<string>.SuccessAsync(data: token);
        }

        return await Result<string>.SuccessAsync(data: user.Id.ToString());
    }

    private async Task<string> SendCode(ApplicationUser user, CancellationToken cancellationToken)
    {
        string? token = await _userManger.GenerateTwoFactorTokenAsync(user, ZeroFatProviders.FourDigitPhoneProvider);
        return token;
    }

    private async Task Update(ApplicationUser user, string code, string? email, string? phone)
    {
        bool isConfirm = await _userManger.VerifyTwoFactorTokenAsync(user, ZeroFatProviders.FourDigitPhoneProvider, code);
        if (!isConfirm)
            throw new BadRequestException(_localizer["Wrong code"]);

        if (email.HasValue())
        {
            user.Email = email;
        }

        if (phone.HasValue())
        {
            user.PhoneNumber = phone;
        }
        await _userManger.UpdateAsync(user);
        await _clientService.UpdateClientOrEmail(user.PublicId, email, phone);
    }
}
