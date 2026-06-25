using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.Users.Application.Contracts;
public interface IUserModule : IScopedService
{
    Task ExecuteCommandAsync(ICommand command);
    Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command);
    Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query);
}
