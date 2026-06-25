using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;

namespace ZeroFat.Users.Application.Users;

public class DeleteUserRequest(string id) : ICommand<Result<string>>
{
    public string Id { get; set; } = id;
}
public class DeleteUserRequestHandler(IStringLocalizer<DeleteUserRequestHandler> localizer, UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteUserRequest, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IStringLocalizer<DeleteUserRequestHandler> _localizer = localizer;

    public async Task<Result<string>> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);

        _ = user ?? throw new NotFoundException(_localizer["User not found"]);

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException(_localizer["Something went wrong"]);

        return await Result<string>.SuccessAsync(user.Id);
    }

}
