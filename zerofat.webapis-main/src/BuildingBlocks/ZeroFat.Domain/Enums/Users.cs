using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ZeroFat.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DeviceType
{
    Android,
    IOS,
    Web
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LoginMechanism
{
    [Description("Password-Based Authentication")]
    PasswordBased,
    [Description("One-Time Password Authentication")]
    OTP,
    [Description("Two-Factor Authentication")]
    TwoFA
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserType
{
    [Description("Administrator User")]
    Admin,
    [Description("Client User")]
    Client,
}
