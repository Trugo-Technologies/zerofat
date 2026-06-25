using ZeroFat.Domain.Common.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Security;
using System.Reflection;

namespace ZeroFat.Users.Domain.Users;
public class ApplicationUserRole : IdentityUserRole<string>, IAggregateRoot
{
    public virtual ApplicationUser User { get; set; } = default!;
    public virtual ApplicationRole? Role { get; set; } = default!;
}

public class ApplicationRoleClaim : IdentityRoleClaim<string>, IAggregateRoot
{
    public string? Action { get; set; }
    public string? Resource { get; set; }
    public string? Module { get; set; }
    public string? SubModule { get; set; }

    public DefaultIdType CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? CreatedByName { get; set; }
}
