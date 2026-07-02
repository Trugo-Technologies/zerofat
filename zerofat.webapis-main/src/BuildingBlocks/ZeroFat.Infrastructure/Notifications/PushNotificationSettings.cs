namespace ZeroFat.Infrastructure.Notifications;

public class PushNotificationSettings
{
    public const string SectionName = "PushNotificationSettings";
    public bool Enabled { get; set; }
    public string? FcmServerKey { get; set; }
}
