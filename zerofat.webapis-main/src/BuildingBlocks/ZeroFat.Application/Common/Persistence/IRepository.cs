using Ardalis.Specification;
using ZeroFat.Domain.Common.Contracts;
using Mapster;
using System.Linq.Expressions;

namespace ZeroFat.Application.Common.Persistence;

// The Repository for the Application Db
// I(Read)RepositoryBase<T> is from Ardalis.Specification

/// <summary>
/// The regular read/write repository for an aggregate root.
/// </summary>
public interface IRepository<T> : IRepositoryBase<T>
    where T : class, IAggregateRoot
{
    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, TypeAdapterConfig config, CancellationToken cancellationToken = default);

    Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, TypeAdapterConfig config, CancellationToken cancellationToken = default);
}

/// <summary>
/// The read-only repository for an aggregate root.
/// </summary>
public interface IReadRepository<T> : IReadRepositoryBase<T>
    where T : class, IAggregateRoot
{
    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, TypeAdapterConfig config, CancellationToken cancellationToken = default);

    Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, TypeAdapterConfig config, CancellationToken cancellationToken = default);
}

/// <summary>
/// A special (read/write) repository for an aggregate root,
/// that also adds EntityCreated, EntityUpdated or EntityDeleted
/// events to the DomainEvents of the entities before adding,
/// updating or deleting them.
/// </summary>
public interface IRepositoryWithEvents<T> : IRepositoryBase<T>
    where T : class, IAggregateRoot
{
    Task<T> AddAsync(T entity, bool withSaveChanges = true, bool withAuditing = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, bool withSaveChanges = true, bool withAuditing = true, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, bool withSaveChanges = true, bool withAuditing = true, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, bool withSaveChanges = true, bool withAuditing = true, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(bool withAuditing, CancellationToken cancellationToken = default);
    Task<T> DeleteAsync(T entity, bool withSaveChanges = true, bool withTracking = true, CancellationToken cancellationToken = default);
    //Task<T> AddAsync(T entity, bool withSaveChanges = true, bool withTracking = true, CancellationToken cancellationToken = default);
    //Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, bool withSaveChanges = true, bool withTracking = true, CancellationToken cancellationToken = default);
    //Task<T> UpdateAsync(T entity, bool withSaveChanges = true, bool withTracking = true, CancellationToken cancellationToken = default);
    //Task UpdateRangeAsync(IEnumerable<T> entities, bool withSaveChanges = true, bool withTracking = true, CancellationToken cancellationToken = default);
    //Task DeleteRangeAsync(IEnumerable<T> entities, bool withSaveChanges = true, bool withTracking = true, CancellationToken cancellationToken = default);
    //Task<int> SaveChangesAsync(bool withAuditing, CancellationToken cancellationToken = default);
    //Task<bool> ExistsAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
    //Task<T?> FirstByConditionAsync(Expression<Func<T, bool>> expression, Expression<Func<T, object?>>? orderExpression = null, CancellationToken cancellationToken = default);
    //Task<TResult?> FirstByConditionAsync<TResult>(Expression<Func<T, bool>> expression, Expression<Func<T, object?>>? orderExpression = null, CancellationToken cancellationToken = default);
    //Task<int> CountByConditionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
    //Task<List<TResult>> FindByConditionAsync<TResult>(Expression<Func<T, bool>> expression, Expression<Func<T, object?>>? orderExpression = null, CancellationToken cancellationToken = default);
    //Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, Expression<Func<T, object?>>? orderExpression = null, CancellationToken cancellationToken = default);
}
