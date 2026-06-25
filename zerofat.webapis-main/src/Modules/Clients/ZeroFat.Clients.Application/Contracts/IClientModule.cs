using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.ClientPortal.Application.Contracts;
public interface IClientPortalModule : IScopedService
{
    Task ExecuteCommandAsync(ICommand command);
    Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command);
    Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query);
}
