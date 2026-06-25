using ZeroFat.Domain.Enums;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.Users.Application.Roles;

public class RoleDto : IDto
{
    public string Id { get; } = default!;
    public string? Description { get; init; }
    public UserType UserType { get; init; }

    public DateTime CreatedOn { get; set; }
    public string? CreatedByName { get; set; }
}
