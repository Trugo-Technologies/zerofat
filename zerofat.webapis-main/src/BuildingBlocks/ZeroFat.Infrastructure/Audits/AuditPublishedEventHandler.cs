using MediatR;
using Microsoft.Extensions.Logging;
using ZeroFat.Application.Audits;
using ZeroFat.Infrastructure.Audits.Persistence;

namespace ZeroFat.Infrastructure.Audits;
public class AuditPublishedEventHandler(
    ILogger<AuditPublishedEventHandler> logger,
    AuditContext context) : INotificationHandler<AuditPublishedEvent>
{
    public async Task Handle(AuditPublishedEvent notification, CancellationToken cancellationToken)
    {
        if (context == null)
            return;
        logger.LogInformation("received audit trails");
        try
        {
            //await context.AuditTrails.InsertManyAsync(notification.Trails!,cancellationToken:cancellationToken);
            await context.AuditTrails.AddRangeAsync(notification.Trails!, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "error while saving audit trail");
        }
        return;
    }
}
