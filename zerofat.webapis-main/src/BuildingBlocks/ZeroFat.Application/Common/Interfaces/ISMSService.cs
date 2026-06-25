namespace ZeroFat.Application.Common.Interfaces;

public interface ISMSService
{
    Task SendAsync(string phoneNumber, string message, bool isTransactional = true, CancellationToken ct = default);
    Task<bool> SendMulticast(string message, List<string?>? phoneNumbers, bool isTransactional = true, CancellationToken cancellationToken = default);
}
