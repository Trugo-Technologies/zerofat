namespace ZeroFat.Domain.Common.Contracts;

public interface IEntity : IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    IReadOnlyCollection<IFileEntry> FileAttachments { get; }

    void ClearDomainEvents();
    void ClearFileAttachments();
    void AddFileAttachment(IFileEntry fileEntry);
}

public interface IEntity<TId> : IEntity
{
    TId Id { get; }
}
