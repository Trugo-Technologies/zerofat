using FluentValidation;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Users.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ZeroFat.Users.Application.Roles;
public class AssignUsersToRoleRequest : ICommand<Result>
{
    public string? RoleName { get; set; } = default!;
    public List<string> UserIds { get; set; }
}

public class AssignUsersToRoleRequestValidator : CustomValidator<AssignUsersToRoleRequest>
{
    public AssignUsersToRoleRequestValidator(
        IReadRepository<ApplicationUser> userRepository,
        IReadRepository<ApplicationRole> roleRepository)
    {
        RuleFor(x => x.RoleName)
            .MustAsync(async (roleName, _) => await roleRepository.AnyAsync(new RoleByNameSpec(roleName), _))
                .WithMessage((_, roleName) => string.Format("Role {0} Not Found.", roleName));
    }
}


public class AssignUsersToRoleRequestHandler : ICommandHandler<AssignUsersToRoleRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICurrentUser _currentUser;

    public AssignUsersToRoleRequestHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ICurrentUser currentUser)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(AssignUsersToRoleRequest request, CancellationToken cancellationToken)
    {
        // if (_currentUser.GetUserId() == Guid.Parse(request.UserId))
        //     throw new BadRequestException(_localizer["user.cannotChangeYourSelfRoles"]);

        foreach (var userId in request.UserIds)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, request.RoleName);
            }
        }

        return (Result)await Result.SuccessAsync();
    }

}
