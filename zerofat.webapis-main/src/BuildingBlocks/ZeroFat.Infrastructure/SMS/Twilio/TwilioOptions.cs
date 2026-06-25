namespace ZeroFat.Infrastructure.SMS.Twilio;

public class TwilioOptions
{
    public string? FromPhoneNumber { get; set; }
    public string? AccountSid { get; set; }
    public string? AuthToken { get; set; }
    public string? MessagingServiceSid { get; set; }
}
