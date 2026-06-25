using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ZeroFat.Users.Application.Roles;
public class CreateRoleRequest : ICommand<Result<string>>
{
    public string Name { get; set; } = default!;
    public UserType UserType { get; set; } = default!;
    public bool? IsActive { get; set; }
    public string? Description { get; set; }
}

internal class CreateRoleRequestValidator : CustomValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator(
        IReadRepository<ApplicationRole> roleRepository)
    {
        RuleFor(r => r.Name)
             .NotEmpty()
             .MustAsync(async (roleName, _) => await roleRepository.AnyAsync(new RoleByNameSpec(roleName), _))
                 .WithMessage("Similar Role already exists");
    }
}

internal class CreateRoleRequestHandler : ICommandHandler<CreateRoleRequest, Result<string>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly RoleManager<ApplicationRole> _roleManager;

    public CreateRoleRequestHandler(RoleManager<ApplicationRole> roleManager) => (_roleManager) = (roleManager);

    public async Task<Result<string>> Handle(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = new ApplicationRole()
        {
            Name = request.Name,
            Description = request.Description,
            UserType = request.UserType,
        };

        // role.DomainEvents.Add(new ApplicationRoleCreatedEvent(role.Id, role.Name));
        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            throw new ValidationException("Register role failed", result.Errors.ToList().ConvertAll(x => new FluentValidation.Results.ValidationFailure(x.Code, x.Description)));
        }

        return await Result<string>.SuccessAsync(string.Format("Role {0} has been created", request.Name));
    }

}
