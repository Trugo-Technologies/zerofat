using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.Infrastructure.Email;

/// <summary>
/// Sends subscription emails (payment link, expiry). Configure SMTP in Configurations/email.json.
/// When EmailSettings:Enabled is false, logs only.
/// </summary>
public class SmtpEmailNotificationService(
    IOptions<EmailSettings> emailOptions,
    ILogger<SmtpEmailNotificationService> logger) : IEmailNotificationService, ISendSubscriptionPaymentLinkEmailJob
{
    private readonly EmailSettings _settings = emailOptions.Value;

    public Task SendSubscriptionPaymentLinkAsync(
        string toEmail,
        string clientName,
        string paymentLink,
        string? optionalMessage,
        CancellationToken cancellationToken = default)
    {
        var subject = "Complete your ZeroFat subscription payment";
        var body = $"""
            <p>Hi {WebUtility.HtmlEncode(clientName)},</p>
            <p>Your subscription is ready. Please complete payment using the link below:</p>
            <p><a href="{WebUtility.HtmlEncode(paymentLink)}">Pay now</a></p>
            {(string.IsNullOrWhiteSpace(optionalMessage) ? "" : $"<p>{WebUtility.HtmlEncode(optionalMessage)}</p>")}
            <p>Thank you,<br/>ZeroFat Team</p>
            """;

        return SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    public Task SendSubscriptionExpiredAsync(
        string toEmail,
        string clientName,
        string subscriptionType,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        var subject = "Your ZeroFat subscription has expired";
        var body = $"""
            <p>Hi {WebUtility.HtmlEncode(clientName)},</p>
            <p>Your <strong>{WebUtility.HtmlEncode(subscriptionType)}</strong> subscription ended on <strong>{endDate:yyyy-MM-dd}</strong>.</p>
            <p>Renew your subscription in the app to continue receiving your meals.</p>
            <p>Thank you,<br/>ZeroFat Team</p>
            """;

        return SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    public Task SendAsync(
        string toEmail,
        string clientName,
        string paymentLink,
        string? optionalMessage,
        CancellationToken cancellationToken = default) =>
        SendSubscriptionPaymentLinkAsync(toEmail, clientName, paymentLink, optionalMessage, cancellationToken);

    private Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        if (!_settings.Enabled)
        {
            logger.LogInformation(
                "Email disabled — would send '{Subject}' to {Email}",
                subject,
                toEmail);
            return Task.CompletedTask;
        }

        if (string.IsNullOrWhiteSpace(_settings.SmtpHost) || string.IsNullOrWhiteSpace(_settings.FromAddress))
        {
            logger.LogWarning(
                "Email enabled but SMTP host/from address missing — skipped '{Subject}' to {Email}",
                subject,
                toEmail);
            return Task.CompletedTask;
        }

        return SendSmtpAsync(toEmail, subject, htmlBody, cancellationToken);
    }

    private async Task SendSmtpAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_settings.FromAddress!, _settings.FromName ?? "ZeroFat"),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
        {
            EnableSsl = _settings.UseSsl,
            Credentials = string.IsNullOrWhiteSpace(_settings.SmtpUsername)
                ? null
                : new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword)
        };

        await client.SendMailAsync(message, cancellationToken);
        logger.LogInformation("Sent email '{Subject}' to {Email}", subject, toEmail);
    }
}
