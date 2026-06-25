using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Domain.ClientManagement;

public class ClientLoyaltyPoint : AuditableEntity, IAggregateRoot
{
    public int Points { get; set; }

    public DateTime DateEarned { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public TransactionType Type { get; set; } // Earn or Consume

    public DefaultIdType ClientId { get; set; }
    public virtual Client Client { get; set; } = default!;

    public string? Reason { get; set; }

    public string? TransactionId { get; set; } // Unique transaction identifier
    public string? Source { get; set; } // Source of the points (e.g., "Purchase", "Referral", "Email Verification")

}
