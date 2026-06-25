using ZeroFat.Infrastructure.SMS.Etisalat;
using ZeroFat.Infrastructure.SMS.Twilio;

namespace ZeroFat.Infrastructure.SMS;

public class SMSOptions
{
    public string? Provider { get; set; }

    public TwilioOptions Twillo { get; set; }

    public EtisalatOptions Etisalat { get; set; }

    public bool UsedTwillo()
    {
        return Provider == "Twillo";
    }

    public bool UsedAmazon()
    {
        return Provider == "Etisalat";
    }

    public bool UsedFake()
    {
        return Provider == "Fake";
    }

}
