using Hangfire;
using Microsoft.Extensions.Logging;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.Infrastructure.Notifications;

/// <summary>
/// Hangfire job: sends subscription-expired email and push notification to the client.
/// </summary>
public class SubscriptionExpiredNotificationService(
    IEmailNotificationService emailService,
    IPushNotificationService pushNotificationService,
    ILogger<SubscriptionExpiredNotificationService> logger) : ISendSubscriptionExpiredNotificationJob
{
    [Queue("subscription-meals")]
    [AutomaticRetry(Attempts = 2)]
    public async Task SendAsync(
        Guid clientId,
        string toEmail,
        string clientName,
        string subscriptionType,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Sending subscription expired notifications for client {ClientId} ({Email}), ended {EndDate}",
            clientId,
            toEmail,
            endDate);

        if (!string.IsNullOrWhiteSpace(toEmail))
        {
            await emailService.SendSubscriptionExpiredAsync(
                toEmail,
                clientName,
                subscriptionType,
                endDate,
                cancellationToken);
        }

        var title = "Subscription expired";
        var body = $"Hi {clientName}, your {subscriptionType} subscription ended on {endDate:yyyy-MM-dd}. Renew now to continue your meal plan.";

        await pushNotificationService.SendToUserAsync(
            clientId,
            title,
            body,
            new Dictionary<string, string>
            {
                ["type"] = "subscription_expired",
                ["clientId"] = clientId.ToString(),
                ["endDate"] = endDate.ToString("yyyy-MM-dd"),
                ["subscriptionType"] = subscriptionType
            },
            cancellationToken);
    }
}
