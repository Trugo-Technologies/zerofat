namespace ZeroFat.Infrastructure.SMS.Etisalat;

public class EtisalatOptions
{
    // API Endpoints
    public string? LoginEndpoint { get; set; }
    public string? SendSmsEndpoint { get; set; }

    // Credentials
    public string? Username { get; set; }
    public string? Password { get; set; }

    // Transactional SMS Config
    public string? TransactionSenderId { get; set; }
    public string? TransactionCategoryId { get; set; }

    // Promotional SMS Config
    public string? PromotionalSenderId { get; set; }
    public string? PromotionalCategoryId { get; set; }
}
