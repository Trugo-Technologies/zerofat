using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Infrastructure.Notifications;
using ZeroFat.Users.Infrastructure.Persistence.Context;

namespace ZeroFat.Users.Infrastructure.Notifications;

/// <summary>
/// Sends FCM push notifications to all devices registered for a user (UserPublicId = client id).
/// Configure PushNotificationSettings in pushnotification.json.
/// </summary>
public class FcmPushNotificationService(
    UsersContext usersContext,
    IHttpClientFactory httpClientFactory,
    IOptions<PushNotificationSettings> pushOptions,
    ILogger<FcmPushNotificationService> logger) : IPushNotificationService
{
    private const string FcmLegacyEndpoint = "https://fcm.googleapis.com/fcm/send";

    public async Task SendToUserAsync(
        Guid userId,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        var settings = pushOptions.Value;
        if (!settings.Enabled)
        {
            logger.LogInformation(
                "Push disabled — would notify user {UserId}: {Title}",
                userId,
                title);
            return;
        }

        if (string.IsNullOrWhiteSpace(settings.FcmServerKey))
        {
            logger.LogWarning("Push enabled but FcmServerKey is missing — skipped notification for user {UserId}", userId);
            return;
        }

        var tokens = await usersContext.Devices
            .AsNoTracking()
            .Where(d => d.UserPublicId == userId && d.FcmToken != null && d.FcmToken != "")
            .Select(d => d.FcmToken!)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (tokens.Count == 0)
        {
            logger.LogInformation("No FCM tokens found for user {UserId}", userId);
            return;
        }

        var payload = new FcmLegacyMessage
        {
            RegistrationIds = tokens,
            Notification = new FcmNotification { Title = title, Body = body },
            Data = data
        };

        var httpClient = httpClientFactory.CreateClient(nameof(FcmPushNotificationService));
        using var request = new HttpRequestMessage(HttpMethod.Post, FcmLegacyEndpoint);
        request.Headers.TryAddWithoutValidation("Authorization", $"key={settings.FcmServerKey}");
        request.Content = JsonContent.Create(payload);

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning(
                "FCM push failed for user {UserId}. Status {Status}. Response: {Response}",
                userId,
                response.StatusCode,
                responseBody);
            return;
        }

        logger.LogInformation(
            "FCM push sent to {TokenCount} device(s) for user {UserId}: {Title}",
            tokens.Count,
            userId,
            title);
    }

    private sealed class FcmLegacyMessage
    {
        [JsonPropertyName("registration_ids")]
        public List<string> RegistrationIds { get; set; } = [];

        [JsonPropertyName("notification")]
        public FcmNotification? Notification { get; set; }

        [JsonPropertyName("data")]
        public IReadOnlyDictionary<string, string>? Data { get; set; }
    }

    private sealed class FcmNotification
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;
    }
}
