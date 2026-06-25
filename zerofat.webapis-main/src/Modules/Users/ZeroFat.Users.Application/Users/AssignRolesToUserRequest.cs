using System.Text.Json.Serialization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Users.Application.Roles;

namespace ZeroFat.Users.Application.Users;
public class AssignRolesToUserRequest : ICommand<Result>
{
    [JsonIgnore]
    public Guid? UserId { get; set; } = default!;
    public List<string> RoleNames { get; set; }
}

public class AssignRolesToUserRequestValidator : CustomValidator<AssignRolesToUserRequest>
{
    public AssignRolesToUserRequestValidator(
        IReadRepository<ApplicationUser> userRepository,
        IReadRepository<ApplicationRole> roleRepository)
    {
        RuleForEach(x => x.RoleNames)
            .MustAsync(async (roleName, _) => await roleRepository.AnyAsync(new RoleByNameSpec(roleName), _))
                .WithMessage((_, roleName) => string.Format("Role {0} Not Found.", roleName));
    }
}


public class AssignRolesToUserRequestHandler : ICommandHandler<AssignRolesToUserRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICurrentUser _currentUser;

    public AssignRolesToUserRequestHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ICurrentUser currentUser)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(AssignRolesToUserRequest request, CancellationToken cancellationToken)
    {
        // if (_currentUser.GetUserId() == Guid.Parse(request.UserId))
        //     throw new BadRequestException(_localizer["user.cannotChangeYourSelfRoles"]);

        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PublicId == request.UserId, cancellationToken);
        if (user == null)
            throw new BadRequestException("User not found");

        await _userManager.AddToRolesAsync(user, request.RoleNames);

        return (Result)await Result.SuccessAsync();
    }

}
