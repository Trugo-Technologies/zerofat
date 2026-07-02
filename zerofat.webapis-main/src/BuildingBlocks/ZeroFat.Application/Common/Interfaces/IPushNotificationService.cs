namespace ZeroFat.Application.Common.Interfaces;

/// <summary>
/// Mobile push notifications via FCM (Configurations/pushnotification.json).
/// </summary>
public interface IPushNotificationService : ITransientService
{
    Task SendToUserAsync(
        Guid userId,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data = null,
        CancellationToken cancellationToken = default);
}
