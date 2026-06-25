using ZeroFat.Application.Common.Models;

namespace ZeroFat.Application.Common.CQRS;

public interface ICommand<out T> : IRequest<T> where T : notnull
{
}
public interface ICommand : ICommand<Result<Guid>>
{
}
