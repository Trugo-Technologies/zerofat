using Hangfire;
using ZeroFat.Infrastructure.BackgroundProcessing.Contracts;

namespace ZeroFat.Infrastructure.BackgroundProcessing;

public class ZerofatJobScheduler : IZerofatJobScheduler
{
    private readonly IEnumerable<IRecurringBackgroundJobScheduler> _jobs;
    private readonly IRecurringJobManager _recurringJobManager;

    public ZerofatJobScheduler(
        IEnumerable<IRecurringBackgroundJobScheduler> jobs,
        IRecurringJobManager recurringJobManager)
    {
        _jobs = jobs;
        _recurringJobManager = recurringJobManager;
    }

    public async Task ScheduleAllRecurringJobsAsync()
    {
        foreach (var job in _jobs)
        {
            await job.ScheduleAsync(_recurringJobManager);
        }
    }
}
