namespace ZeroFat.Application.Common.Models;

public class StripePaymentMethodDto
{
    /// <summary>
    /// Stripe PaymentMethod ID
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Type of payment method (card, sepa_debit, etc.)
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// The ID of the customer this payment method is attached to
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// When the payment method was created
    /// </summary>
    public DateTime Created { get; set; }

    // Card-specific details (if type is "card")
    public CardDetailsDto? Card { get; set; }

    // Add other payment method type details as needed (BankAccount, etc.)
}

public class CardDetailsDto
{
    /// <summary>
    /// Card brand (Visa, MasterCard, etc.)
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Last 4 digits of the card
    /// </summary>
    public string? Last4 { get; set; }

    /// <summary>
    /// Two-digit number representing the card's expiration month
    /// </summary>
    public int ExpMonth { get; set; }

    /// <summary>
    /// Four-digit number representing the card's expiration year
    /// </summary>
    public int ExpYear { get; set; }

    /// <summary>
    /// Cardholder name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Funding type (credit, debit, prepaid, etc.)
    /// </summary>
    public string? Funding { get; set; }
}


public class StripeCouponDto
{
    public string? Id { get; set; }  // e.g., "promo_XYZ123"
    public string? Name { get; set; }
    public string? Code { get; set; }  // e.g., "SUMMER2024"
    public long? AmountOff { get; set; }
    public decimal? PercentOff { get; set; }
    public string? Duration { get; set; }
    public int? DurationInMonths { get; set; }
    public DateTime? RedeemBy { get; set; }
    public bool IsValid { get; set; }  // Stripe's `valid` flag
    public int TimesRedeemed { get; set; }
    public int? MaxRedemptions { get; set; }
}
