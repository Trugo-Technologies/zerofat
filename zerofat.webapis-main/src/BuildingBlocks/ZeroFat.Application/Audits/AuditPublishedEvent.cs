using System.Collections.ObjectModel;
using ZeroFat.Domain.Audits;

namespace ZeroFat.Application.Audits;
public class AuditPublishedEvent : INotification
{
    public AuditPublishedEvent(Collection<AuditTrail>? trails)
    {
        Trails = trails;
    }
    public Collection<AuditTrail>? Trails { get; }
}
