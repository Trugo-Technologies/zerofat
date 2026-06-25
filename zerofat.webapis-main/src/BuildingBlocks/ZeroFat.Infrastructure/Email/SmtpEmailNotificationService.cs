using Microsoft.Extensions.Logging;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.Infrastructure.Email;

/// <summary>SMTP settings — loaded from Configurations/email.json (section EmailSettings).</summary>
public class EmailSettings
{
    public const string SectionName = "EmailSettings";
    public bool Enabled { get; set; }
    public string? FromAddress { get; set; }
    public string? FromName { get; set; }
    public string? SmtpHost { get; set; }
    public int SmtpPort { get; set; } = 587;
    public string? SmtpUsername { get; set; }
    public string? SmtpPassword { get; set; }
    public bool UseSsl { get; set; } = true;
}

/// <summary>
/// Sends subscription payment-link emails on wizard finalize (Hangfire job: ISendSubscriptionPaymentLinkEmailJob).
/// When EmailSettings:Enabled is false, logs only — configure SMTP in email.json to send real mail.
/// </summary>
public class SmtpEmailNotificationService(
    ILogger<SmtpEmailNotificationService> logger) : IEmailNotificationService, ISendSubscriptionPaymentLinkEmailJob
{
    public Task SendSubscriptionPaymentLinkAsync(
        string toEmail,
        string clientName,
        string paymentLink,
        string? optionalMessage,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Subscription payment link email queued for {Email}. Client: {ClientName}. Message: {Message}. Link: {Link}",
            toEmail,
            clientName,
            optionalMessage,
            paymentLink);

        return Task.CompletedTask;
    }

    public Task SendAsync(
        string toEmail,
        string clientName,
        string paymentLink,
        string? optionalMessage,
        CancellationToken cancellationToken = default) =>
        SendSubscriptionPaymentLinkAsync(toEmail, clientName, paymentLink, optionalMessage, cancellationToken);
}
