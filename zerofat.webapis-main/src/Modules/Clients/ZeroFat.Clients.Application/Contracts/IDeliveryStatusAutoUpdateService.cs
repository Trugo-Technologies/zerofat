using Hangfire;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.ClientPortal.Application.Contracts;

/// <summary>
/// Automatically transitions daily selections from Pending to Delivered
/// after the preferred delivery time window has ended.
/// </summary>
public interface IDeliveryStatusAutoUpdateService : ITransientService
{
    [Queue("subscription-meals")]
    [AutomaticRetry(Attempts = 1)]
    Task MarkDeliveriesAsDeliveredAsync(CancellationToken cancellationToken = default);
}
