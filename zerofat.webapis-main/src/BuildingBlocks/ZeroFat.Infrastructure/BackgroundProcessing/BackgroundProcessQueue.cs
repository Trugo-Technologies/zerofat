namespace ZeroFat.Infrastructure.BackgroundProcessing;

using System.Linq.Expressions;
using Hangfire;
using ZeroFat.Infrastructure.BackgroundProcessing.Contracts;

internal sealed class BackgroundProcessQueue : IBackgroundProcessQueue
{
    public void Enqueue(Expression<Func<Task>> methodCall) => BackgroundJob.Enqueue(methodCall);
}
