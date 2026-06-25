using ZeroFat.Application.Common.Models;

namespace ZeroFat.Application.Common.CQRS;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
 where TCommand : ICommand<TResponse>
 where TResponse : notnull
{
}

public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Result<Guid>>
    where TCommand : ICommand
{
}

