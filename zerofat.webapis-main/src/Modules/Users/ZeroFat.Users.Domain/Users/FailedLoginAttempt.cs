namespace ZeroFat.Users.Domain.Users;

public class FailedLoginAttempt : Entity<long>, IAggregateRoot
{
    public string? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; }
    public DateTime LoginDate { get; set; }

}
