using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.Infrastructure.SMS.Twilio;

public class TwilioService : ISMSService
{
    private readonly TwilioOptions _settings;

    public TwilioService(TwilioOptions settings)
    {
        _settings = settings;
    }

    public async Task SendAsync(string phoneNumber, string message, bool isTransactional = true, CancellationToken ct = default)
    {
        //if (_settings == null || !_settings.Enabled)
        //    return;
        //try
        //{
        //    TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);

        //    await MessageResource.CreateAsync(
        //        body: message,
        //        messagingServiceSid: _settings.MessagingServiceSid, // <-- Key change is here
        //        to: new Twilio.Types.PhoneNumber(phoneNumber));
        //}
        //catch(Exception e)
        //{
        //    _logger.LogError(e, e.Message);
        //}
    }

    public async Task<bool> SendMulticast(string message, List<string?>? phoneNumbers, bool isTransactional = true, CancellationToken cancellationToken = default)
    {

        //if (phoneNumbers == null || phoneNumbers.Count == 0 || !_settings.Enabled)
        //    return true;

        //try
        //{
        //    TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);

        //    foreach (string? phoneNumber in phoneNumbers)
        //    {
        //        await MessageResource.CreateAsync(
        //                body: message,
        //                messagingServiceSid: _settings.MessagingServiceSid, // <-- Key change is here
        //                to: new Twilio.Types.PhoneNumber(phoneNumber));
        //    }
        //}
        //catch
        //{
        //    return false;
        //}

        return true;
    }

}
