using MassTransit;

namespace ZeroFat.Domain.Common.Contracts;
public abstract class Entity<TId> : IEntity<TId>
{

    public virtual TId Id { get; set; } = default!;

    private List<IDomainEvent>? _domainEvents;
    private List<IFileEntry>? _fileAttachments;

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly() ?? new List<IDomainEvent>().AsReadOnly();
    public IReadOnlyCollection<IFileEntry> FileAttachments => _fileAttachments?.AsReadOnly() ?? new List<IFileEntry>().AsReadOnly();


    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents ??= new List<IDomainEvent>();

        _domainEvents.Add(domainEvent);
    }

    public void ClearFileAttachments()
    {
        _fileAttachments?.Clear();
    }

    public void AddFileAttachment(IFileEntry fileEntry)
    {
        _fileAttachments ??= new List<IFileEntry>();

        _fileAttachments.Add(fileEntry);
    }
}

public abstract class Entity : Entity<DefaultIdType>
{
    protected Entity()
    {
        Id = NewId.Next().ToGuid();
    }
}
