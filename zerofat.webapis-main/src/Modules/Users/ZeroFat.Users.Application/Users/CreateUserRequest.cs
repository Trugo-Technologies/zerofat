using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Users.Domain;
using ZeroFat.Domain.Enums;
using ZeroFat.Shared.Authorization;
using ZeroFat.Users.Application.Common.Extensions;

namespace ZeroFat.Users.Application.Users;
public class CreateUserRequest : ICommand<Result<string>>
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public IFormFile? Image { get; set; }
    [JsonIgnore]
    public UserType? UserType { get; set; }
}

public class CreateUserRequestValidator : CustomValidator<CreateUserRequest>
{
    public CreateUserRequestValidator(IReadRepository<ApplicationUser> userRepository, IReadRepository<ApplicationRole> roleRepository)
    {

        RuleFor(u => u.Email).Cascade(CascadeMode.Stop)
             .NotNull()
             .EmailAddress()
                 .WithMessage("Invalid Email Address.")
             .MustAsync(async (email, _) => !await userRepository.AnyAsync(new UserByEmailSpec(email), _))
                 .WithMessage((_, email) => string.Format("Email {0} is already registered.", email));

        RuleFor(u => u.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
            .MustAsync(async (phone, _) => !await userRepository.AnyAsync(new UserByPhoneNumberSpec(phone), _))
               .WithMessage((_, phone) => string.Format("Phone number {0} is already registered.", phone));

        RuleFor(p => p.Password).Cascade(CascadeMode.Stop)
        .NotEmpty()
        .MinimumLength(6);
    }
}

public class CreateUserRequestHandler : IRequestHandler<CreateUserRequest, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFileStorageManager _fileStorage;
    private readonly IStringLocalizer<CreateUserRequestHandler> _localizer;

    public CreateUserRequestHandler(UserManager<ApplicationUser> userManager, IStringLocalizer<CreateUserRequestHandler> localizer, IFileStorageManager fileStorage)
    {
        _userManager = userManager;
        _localizer = localizer;
        _fileStorage = fileStorage;
    }

    public async Task<Result<string>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            IsActive = request.IsActive,
            UserName = request.PhoneNumber,
            ImageUrl = await _fileStorage.UploadAsync<ApplicationUser>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new InternalServerException(_localizer["Validation Errors Occurred."], result.GetErrors(_localizer));

        if (request.UserType == UserType.Admin)
            await _userManager.AddToRoleAsync(user, ZeroFatRoles.Admin);
        else
            await _userManager.AddToRoleAsync(user, ZeroFatRoles.Client);

        return await Result<string>.SuccessAsync(data: user.Id);
    }

}
