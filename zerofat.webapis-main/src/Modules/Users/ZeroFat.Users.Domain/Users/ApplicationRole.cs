using ZeroFat.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ZeroFat.Users.Domain.Users;
public class ApplicationRole : IdentityRole, IAuditableEntity, ISoftDelete, IAggregateRoot
{
    public ApplicationRole()
    {
        ApplicationUserRoles = new List<ApplicationUserRole>();
    }
    public string? Description { get; set; }
    public UserType UserType { get; set; }
    public virtual ICollection<ApplicationUserRole> ApplicationUserRoles { get; set; }

    public DefaultIdType CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? CreatedByName { get; set; }
    public DefaultIdType LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string? LastModifiedByName { get; set; }
    public DateTime? DeletedOn { get; set; }
    public DefaultIdType? DeletedBy { get; set; }
    public string? DeletedByName { get; set; }
}
