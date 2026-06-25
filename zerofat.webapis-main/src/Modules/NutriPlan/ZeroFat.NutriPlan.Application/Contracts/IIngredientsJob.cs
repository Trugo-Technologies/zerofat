using Hangfire;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.NutriPlan.Application.Contracts;
public interface IIngredientsJob : ITransientService
{
    [Queue("default")]
    [AutomaticRetry(Attempts = 0)]
    Task GetUSDAIngredientsAsync();
}
