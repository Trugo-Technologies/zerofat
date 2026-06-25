using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Users.Application.Users;

public class UserDto : IDto
{
    public string? Id { get; set; }
    public Guid PublicId { get; set; }

    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public string? ImageUrl { get; set; }

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public UserType UserType { get; set; }

    public bool IsActive { get; set; }
    public List<string>? Roles { get; set; }

    public DateTime CreatedOn { get; set; }
    public string? CreatedByName { get; set; }
}
