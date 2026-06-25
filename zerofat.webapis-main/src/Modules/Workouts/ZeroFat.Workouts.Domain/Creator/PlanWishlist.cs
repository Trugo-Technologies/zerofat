namespace ZeroFat.GymUp.Domain.Creator;

public class PlanWishlist : AuditableEntity, IAggregateRoot
{
    public DefaultIdType PlanId { get; set; }
    public DefaultIdType UserId { get; set; }
    public Plan? Plan { get; set; }
}
