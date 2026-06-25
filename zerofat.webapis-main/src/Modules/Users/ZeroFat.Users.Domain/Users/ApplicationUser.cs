using ZeroFat.Domain.Enums;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace ZeroFat.Users.Domain.Users;
public class ApplicationUser : IdentityUser, IAuditableEntity, ISoftDelete, IHaveActivation, IAggregateRoot
{
    public ApplicationUser()
    {
        ApplicationUserRoles = new List<ApplicationUserRole>();
        FailedLoginAttempts = new List<FailedLoginAttempt>();
        PasswordHistories = new List<PasswordHistory>();
        PublicId = NewId.Next().ToGuid();
    }

    public string? FullName { get; set; }
    public string? ImageUrl { get; set; }
    public Guid PublicId { get; set; }
    public LoginMechanism? LoginMechanism { get; set; }
    public UserType UserType { get; set; }
    public virtual ICollection<ApplicationUserRole> ApplicationUserRoles { get;}
    public virtual ICollection<FailedLoginAttempt> FailedLoginAttempts { get; }
    public virtual ICollection<PasswordHistory> PasswordHistories { get; }

    public DefaultIdType CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? CreatedByName { get; set; }
    public DefaultIdType LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string? LastModifiedByName { get; set; }
    public DateTime? DeletedOn { get; set; }
    public DefaultIdType? DeletedBy { get; set; }
    public string? DeletedByName { get; set; }

    public bool IsActive { get; set ; }
    public bool IsTest { get; set; }

    public DateTime? ActivationChangedOn { get; set; }
    public string? ActivationChangedByName { get; set; }
    public DefaultIdType? ActivationChangedBy { get; set; }
}
