using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.Application.Core.Contracts;
public interface ICoreModule : IScopedService
{
    Task ExecuteCommandAsync(ICommand command);
    Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command);
    Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query);
}
