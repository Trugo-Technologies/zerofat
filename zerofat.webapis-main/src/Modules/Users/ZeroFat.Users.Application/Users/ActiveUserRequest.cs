using ZeroFat.Application.Common.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ZeroFat.Users.Application.Users;
public class ActiveUserRequest : ICommand<Result<bool>>
{
    public string Id { get; set; }
    public ActiveUserRequest(string id) => Id = id;
}

public class ActiveUserRequestHandler : ICommandHandler<ActiveUserRequest, Result<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<ActiveUserRequestHandler> _localizer;

    public ActiveUserRequestHandler(UserManager<ApplicationUser> userManager, IStringLocalizer<ActiveUserRequestHandler> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(ActiveUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

        _ = user ?? throw new NotFoundException(_localizer["User not found"]);

        user.IsActive = !user.IsActive;

        await _userManager.UpdateAsync(user);

        return await Result<bool>.SuccessAsync(user.IsActive);
    }
}
