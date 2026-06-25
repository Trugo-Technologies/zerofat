namespace ZeroFat.Users.Domain.Users;

public class PasswordHistory: Entity<long>, IAggregateRoot
{
    public string? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; }

    public string Password { get; set; } = default!;
    public DateTime ChangeDate { get; set; }
}
