using MassTransit;

namespace ZeroFat.Domain.Common.Contracts;

public abstract class ActivationEntity : ActivationEntity<DefaultIdType>
{
    protected ActivationEntity()
    {
        Id = NewId.Next().ToGuid();
    }
}

public abstract class ActivationEntity<T> : AuditableEntity<T>, IHaveActivation
{
    public bool IsActive { get; set; }
    public DateTime? ActivationChangedOn { get; set; }
    public string? ActivationChangedByName { get; set; }
    public DefaultIdType? ActivationChangedBy { get; set; }

    protected ActivationEntity()
    {
    }
}
