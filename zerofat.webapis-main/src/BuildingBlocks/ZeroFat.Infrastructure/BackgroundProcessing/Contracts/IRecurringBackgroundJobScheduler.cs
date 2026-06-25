using Hangfire;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.Infrastructure.BackgroundProcessing.Contracts;

public interface IRecurringBackgroundJobScheduler : ITransientService
{
    void Schedule(IRecurringJobManager recurringJobManager);
}

public interface IZerofatJobScheduler
{
    Task ScheduleAllRecurringJobsAsync();
}

