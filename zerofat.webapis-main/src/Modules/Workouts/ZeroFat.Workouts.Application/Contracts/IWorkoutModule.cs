using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.GymUp.Application.Contracts;
public interface IWorkoutModule : IScopedService
{
    Task ExecuteCommandAsync(ICommand command);
    Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command) where TResult : notnull;
    Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query) where TResult : notnull;
}
