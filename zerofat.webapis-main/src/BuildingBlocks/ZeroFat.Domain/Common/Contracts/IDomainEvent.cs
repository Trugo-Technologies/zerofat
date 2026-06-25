using MediatR;

namespace ZeroFat.Domain.Common.Contracts;

public interface IDomainEvent : INotification
{
    Guid Id { get; }

    DateTime OccurredOn { get; }
}
