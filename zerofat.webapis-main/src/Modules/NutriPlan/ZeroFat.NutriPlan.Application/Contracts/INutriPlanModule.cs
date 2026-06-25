using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.NutriPlan.Application.Contracts;
public interface INutriPlanModule : IScopedService
{
    Task ExecuteCommandAsync(ICommand command);
    Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command);
    Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query);
}
