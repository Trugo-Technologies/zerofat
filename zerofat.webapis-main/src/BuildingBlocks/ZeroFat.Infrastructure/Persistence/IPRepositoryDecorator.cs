using Ardalis.Specification;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Common.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ZeroFat.Infrastructure.Persistence;

/// <summary>
/// The repository that implements IRepositoryWithEvents.
/// Implemented as a decorator. It only augments the Add,
/// Update and Delete calls where it adds the respective
/// EntityCreated, EntityUpdated or EntityDeleted event
/// before delegating to the decorated repository.
/// </summary>
public class IPRepositoryDecorator<T, TContext> : IRepositoryWithEvents<T>
    where T : class, IAggregateRoot
    where TContext : DbContext, IIPDbContext
{
    private readonly IRepository<T> _decorated;
    private readonly TContext _dbContext;

    public IPRepositoryDecorator(TContext dbContext, IRepository<T> decorated)
    {
        _dbContext = dbContext;
        _decorated = decorated;
    }

    #region Commands
    public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        // entity.DomainEvents.Add(EntityCreatedEvent.WithEntity(entity));
        return _decorated.AddAsync(entity, cancellationToken);
    }

    public Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        // foreach (var entity in entities)
        //     entity.DomainEvents.Add(EntityCreatedEvent.WithEntity(entity));
        return _decorated.AddRangeAsync(entities, cancellationToken);
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, bool withSaveChanges = true, bool withAuditing = true, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddRangeAsync(entities, cancellationToken);
        if (withSaveChanges)
            await _dbContext.SaveChangesAsync(withAuditing: withAuditing, cancellationToken: cancellationToken);
        return entities;
    }

    public Task<int> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        // entity.DomainEvents.Add(EntityUpdatedEvent.WithEntity(entity));
        return _decorated.UpdateAsync(entity, cancellationToken);
    }

    public Task<int> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        // foreach (var entity in entities)
        //     entity.DomainEvents.Add(EntityUpdatedEvent.WithEntity(entity));
        return _decorated.UpdateRangeAsync(entities, cancellationToken);
    }

    public Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        // entity.DomainEvents.Add(EntityDeletedEvent.WithEntity(entity));
        return _decorated.DeleteAsync(entity, cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, bool withSaveChanges = true, bool withAuditing = true, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        if (withSaveChanges)
            await _dbContext.SaveChangesAsync(withAuditing: withAuditing, cancellationToken: cancellationToken);
    }

    public async Task<T> DeleteAsync(T entity, bool withSaveChanges = true, bool withAuditing = true, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Remove(entity);

        if (withSaveChanges)
            await _dbContext.SaveChangesAsync(withAuditing: withAuditing, cancellationToken: cancellationToken);
        return entity;
    }

    public Task<int> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        // foreach (var entity in entities)
        // {
        //     entity.DomainEvents.Add(EntityDeletedEvent.WithEntity(entity));
        // }

        return _decorated.DeleteRangeAsync(entities, cancellationToken);
    }


    public async Task<T> AddAsync(T entity, bool withSaveChanges = true, bool withAuditing = true, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        if (withSaveChanges)
            await _dbContext.SaveChangesAsync(withAuditing: withAuditing, cancellationToken: cancellationToken);
        return entity;
    }

    public async Task<T> UpdateAsync(T entity, bool withSaveChanges = true, bool withAuditing = true, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Update(entity);
        if (withSaveChanges)
            await _dbContext.SaveChangesAsync(withAuditing: withAuditing, cancellationToken: cancellationToken);
        return entity;
    }
    #endregion

    public async Task<int> SaveChangesAsync(bool withAuditing, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(withAuditing: withAuditing, cancellationToken: cancellationToken);
    }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _decorated.SaveChangesAsync(cancellationToken);
    public Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        where TId : notnull =>
        _decorated.GetByIdAsync(id, cancellationToken);
    public Task<List<T>> ListAsync(CancellationToken cancellationToken = default) =>
       _decorated.ListAsync(cancellationToken);
    public Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
        _decorated.ListAsync(specification, cancellationToken);
    public Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default) =>
        _decorated.ListAsync(specification, cancellationToken);
    public Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
        _decorated.AnyAsync(specification, cancellationToken);
    public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        _decorated.AnyAsync(cancellationToken);
    public Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
        _decorated.CountAsync(specification, cancellationToken);
    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        _decorated.CountAsync(cancellationToken);
    public Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
            _decorated.FirstOrDefaultAsync(specification, cancellationToken);
    public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default) =>
            _decorated.FirstOrDefaultAsync(specification, cancellationToken);
    public Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default) =>
            _decorated.SingleOrDefaultAsync(specification, cancellationToken);
    public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default) =>
            _decorated.SingleOrDefaultAsync(specification, cancellationToken);
    public IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification) =>
            _decorated.AsAsyncEnumerable(specification);


    public Task<int> DeleteRangeAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
        _decorated.DeleteRangeAsync(specification, cancellationToken);

}
