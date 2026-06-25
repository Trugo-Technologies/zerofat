namespace ZeroFat.Infrastructure.BackgroundProcessing.Contracts;

using System;
using System.Linq.Expressions;

public interface IBackgroundJobScheduler
{
    void Schedule(Expression<Action> methodCall, DateTime date);
    void UnSchedule(Expression<Action> methodCall);
}
