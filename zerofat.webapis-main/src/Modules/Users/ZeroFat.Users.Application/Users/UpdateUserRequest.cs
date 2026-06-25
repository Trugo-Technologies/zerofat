using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.Users.Domain;

namespace ZeroFat.Users.Application.Users;
public class UpdateUserRequest : ICommand<Result<string>>
{
    public string? Id { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? ImageUrl { get; set; }
    public IFormFile? Image { get; set; }

    public bool IsActive { get; set; }
}

public class UpdateUserRequestValidator : CustomValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator(
        IReadRepository<ApplicationUser> userRepository,
        IReadRepository<ApplicationRole> roleRepository
        )
    {

        RuleFor(u => u.Email).Cascade(CascadeMode.Stop)
             .EmailAddress()
                 .WithMessage("Invalid Email Address.")
             .MustAsync(async (request, email, _) => !await userRepository.AnyAsync(new UserByEmailSpec(email, request.Id), _))
                 .WithMessage((_, email) => string.Format("Email {0} is already registered.", email));

        RuleFor(u => u.PhoneNumber).Cascade(CascadeMode.Stop)
         .MustAsync(async (request, phone, _) => !await userRepository.AnyAsync(new UserByPhoneNumberSpec(phone, request.Id), _))
             .WithMessage((_, phone) => string.Format("Phone number {0} is already registered.", phone));

    }
}

public class UpdateUserRequestHandler : IRequestHandler<UpdateUserRequest, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<UpdateUserRequestHandler> _localizer;
    private readonly IFileStorageManager _uploadFile;

    public UpdateUserRequestHandler(UserManager<ApplicationUser> userManager, IStringLocalizer<UpdateUserRequestHandler> localizer, IFileStorageManager uploadFile)
    {
        _userManager = userManager;
        _localizer = localizer;
        _uploadFile = uploadFile;
    }

    public async Task<Result<string>> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.Email = request.Email;
        user.IsActive = request.IsActive;
        user.ImageUrl = request.Image != null ? await _uploadFile.UploadAsync<ApplicationUser>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken) : request.ImageUrl == user.ImageUrl ? user.ImageUrl : null;

        await _userManager.UpdateAsync(user);

        return await Result<string>.SuccessAsync(data: user.Id);
    }

}
