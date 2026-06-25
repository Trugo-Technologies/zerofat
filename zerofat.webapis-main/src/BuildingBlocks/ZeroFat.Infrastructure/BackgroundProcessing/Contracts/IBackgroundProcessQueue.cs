namespace ZeroFat.Infrastructure.BackgroundProcessing.Contracts;

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

public interface IBackgroundProcessQueue
{
    void Enqueue(Expression<Func<Task>> methodCall);
}
