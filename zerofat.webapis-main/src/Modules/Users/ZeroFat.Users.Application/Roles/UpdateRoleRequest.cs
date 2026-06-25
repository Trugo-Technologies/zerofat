using Microsoft.AspNetCore.Identity;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;

namespace ZeroFat.Users.Application.Roles;
public class UpdateRoleRequest : ICommand<Result<string>>
{
    public string Id { get; set; }
    public string Name { get; set; } = default!;
    public bool? IsActive { get; set; }
    public string? Description { get; set; }
}

internal class UpdateRoleRequestValidator : CustomValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator(
        IReadRepository<ApplicationRole> roleRepository)
    {
        RuleFor(r => r.Name)
             .NotEmpty()
             .MustAsync(async (roleName, _) => await roleRepository.AnyAsync(new RoleByNameSpec(roleName), _))
                 .WithMessage("Similar Role already exists");
    }
}

internal class UpdateRoleRequestHandler : ICommandHandler<UpdateRoleRequest, Result<string>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UpdateRoleRequestHandler(RoleManager<ApplicationRole> roleManager) => (_roleManager) = (roleManager);

    public async Task<Result<string>> Handle(UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role == null)
            throw new NotFoundException("Role Not Found");

        role.Name = request.Name;
        role.Description = request.Description;

        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            throw new ValidationException("Update role failed", result.Errors.ToList().ConvertAll(x => new FluentValidation.Results.ValidationFailure(x.Code, x.Description)));
        }

        return await Result<string>.SuccessAsync(string.Format("Role {0} has been Updated", request.Name));
    }

}
