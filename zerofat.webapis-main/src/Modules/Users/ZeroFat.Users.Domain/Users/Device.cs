using ZeroFat.Domain.Enums;

namespace ZeroFat.Users.Domain.Users;

public class Device : Entity, IAggregateRoot
{
    public Device()
    {
        CreatedOn = DateTime.UtcNow;
    }
    public string? BaseDeviceId { get; set; } = default!;
    public string? DeviceModel { get; set; }
    public string? DeviceOs { get; set; }

    public string? FcmToken { get; set; }
    public string? CurrentLanguage { get; set; }

    public int TotalOTPCount { get; set; }
    public int TotalOTPConsecutiveCount { get; set; }

    public DeviceType DeviceType { get; set; }
    public string? Platform { get; set; }
    public string? Version { get; set; }
    public DateTime? LastSeen { get; set; }

    public string? LastVerificationCode { get; set; }
    public DateTime? LastOTPTime { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public Guid? UserPublicId { get; set; }

    /// <summary>
    /// Only On Debug
    /// </summary>
    public string? Token { get; set; }
    public bool IsTrusted { get; set; }
    public DateTime? LastLogin { get; set; }

    public DateTime CreatedOn { get; set; }
    public string? IPAddressOnCreated { get; set; }
    public string? LastKnownIPAddress { get; set; }
}
