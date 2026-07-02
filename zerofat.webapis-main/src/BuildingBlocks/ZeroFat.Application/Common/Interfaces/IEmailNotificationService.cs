namespace ZeroFat.Application.Common.Interfaces;

/// <summary>Email sender — implementation: SmtpEmailNotificationService (Configurations/email.json).</summary>
public interface IEmailNotificationService : ITransientService
{
    Task SendSubscriptionPaymentLinkAsync(
        string toEmail,
        string clientName,
        string paymentLink,
        string? optionalMessage,
        CancellationToken cancellationToken = default);

    Task SendSubscriptionExpiredAsync(
        string toEmail,
        string clientName,
        string subscriptionType,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
}

/// <summary>Hangfire job enqueued on wizard finalize — sends payment link to customer email.</summary>
public interface ISendSubscriptionPaymentLinkEmailJob : ITransientService
{
    Task SendAsync(
        string toEmail,
        string clientName,
        string paymentLink,
        string? optionalMessage,
        CancellationToken cancellationToken = default);
}

/// <summary>Hangfire job enqueued when a client's subscription expires — email + push.</summary>
public interface ISendSubscriptionExpiredNotificationJob : ITransientService
{
    Task SendAsync(
        Guid clientId,
        string toEmail,
        string clientName,
        string subscriptionType,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
}
