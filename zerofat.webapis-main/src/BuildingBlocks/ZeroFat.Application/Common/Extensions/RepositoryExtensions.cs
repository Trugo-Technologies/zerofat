using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Domain.Common.Contracts;

namespace ZeroFat.Application.Common.Extensions;

public static class RepositoryExtensions
{
    public static async Task UpdateRelatedEntitiesAsync<TRelatedEntity, TRequest>(
    this IRepositoryWithEvents<TRelatedEntity> repository,
    DefaultIdType parentId,
    IEnumerable<TRequest> relatedEntityRequests,
    Func<TRequest, DefaultIdType> getRelatedEntityId, // Extracts ID from TRequest
    Func<TRelatedEntity, DefaultIdType> getRelatedEntityIdFromEntity, // Extracts ID from TRelatedEntity
    Func<TRequest, TRelatedEntity, TRelatedEntity> updateExistingEntity,
    Func<TRequest, DefaultIdType, TRelatedEntity> createNewEntity,
    ExpressionSpecification<TRelatedEntity> expressionSpecification,
    CancellationToken cancellationToken)
    where TRelatedEntity : class, IAggregateRoot, IEntity<DefaultIdType>
    {
        // Fetch existing related entities
        var existingRelatedEntities = await repository.ListAsync(expressionSpecification, cancellationToken);

        // Update or add related entities
        foreach (var request in relatedEntityRequests)
        {
            var relatedEntityId = getRelatedEntityId(request);
            var existingRelatedEntity = existingRelatedEntities.FirstOrDefault(x => getRelatedEntityIdFromEntity(x) == relatedEntityId);

            if (existingRelatedEntity != null)
            {
                // Update existing related entity
                var updatedEntity = updateExistingEntity(request, existingRelatedEntity);
                await repository.UpdateAsync(updatedEntity, withSaveChanges: false, cancellationToken: cancellationToken);
            }
            else
            {
                // Add new related entity
                var newRelatedEntity = createNewEntity(request, parentId);
                await repository.AddAsync(newRelatedEntity, withSaveChanges: false, cancellationToken: cancellationToken);
            }
        }

        // Remove related entities that are no longer in the request
        var relatedEntityIdsInRequest = relatedEntityRequests.Select(getRelatedEntityId).ToList();
        var relatedEntitiesToRemove = existingRelatedEntities
            .Where(x => !relatedEntityIdsInRequest.Contains(getRelatedEntityIdFromEntity(x)))
            .ToList();

        if (relatedEntitiesToRemove.Count != 0)
        {
            await repository.DeleteRangeAsync(relatedEntitiesToRemove, withSaveChanges: false, cancellationToken: cancellationToken);
        }
    }

    public static async Task SyncRelationAsync<T>(
    this IRepositoryWithEvents<T> repository,
    ExpressionSpecification<T> expressionSpecification,
    IReadOnlyCollection<Guid>? requestedIds,
    Func<T, Guid> getIdSelector,
    Func<Guid, T> createEntity,
    CancellationToken cancellationToken
) where T : class, IAggregateRoot, IEntity<DefaultIdType>
    {
        var existingEntities = await repository.ListAsync(expressionSpecification, cancellationToken);

        var existingIds = existingEntities.Select(getIdSelector).ToHashSet();
        var requestSet = requestedIds?.ToHashSet() ?? new HashSet<Guid>();

        var toDelete = existingEntities.Where(e => !requestSet.Contains(getIdSelector(e))).ToList();
        var toAdd = requestSet.Except(existingIds).Select(createEntity).ToList();

        if (toDelete.Any())
            await repository.DeleteRangeAsync(toDelete, withSaveChanges: false, cancellationToken: cancellationToken);

        if (toAdd.Any())
            await repository.AddRangeAsync(toAdd, withSaveChanges: false, cancellationToken: cancellationToken);
    }
}
