namespace ZeroFat.Domain.Common.Contracts;

public abstract class DomainEventBase : IDomainEvent
{
    protected DomainEventBase()
    {
        Id = DefaultIdType.NewGuid();
        // OccurredOn = DateTime.UtcNow;
        OccurredOn = SystemTime.Now();
    }

    public DefaultIdType Id { get; }

    public DateTime OccurredOn { get; }
}


