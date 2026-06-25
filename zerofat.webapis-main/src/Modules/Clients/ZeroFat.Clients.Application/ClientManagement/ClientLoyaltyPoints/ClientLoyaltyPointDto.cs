using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientLoyaltyPoints;

public class ClientLoyaltyPointSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public int Points { get; set; }

    public DateTime DateEarned { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public TransactionType Type { get; set; } // Earn or Consume


    public string? Reason { get; set; }

    public string? TransactionId { get; set; } // Unique transaction identifier
    public string? Source { get; set; } // Source of the points (e.g., "Purchase", "Referral", "Email Verification")



}

public class ClientLoyaltyPointRawDto : ClientLoyaltyPointSimplifyDto
{
    public DefaultIdType ClientId { get; set; }
}


public class ClientLoyaltyPointDto : ClientLoyaltyPointRawDto
{
}

