using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Common.Contracts;
using ZeroFat.Domain.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Application.Common.Persistence;
using System.Collections.ObjectModel;
using MediatR;
using ZeroFat.Application.Audits;
using ZeroFat.Domain.Enums;
using ZeroFat.Domain.Audits;

namespace ZeroFat.Infrastructure.Persistence.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser;
    private readonly IPublisher _publisher;
    private readonly IMediator _mediator;

    public AuditableEntityInterceptor(ICurrentUser currentUser, IPublisher publisher, IMediator mediator)
    {
        _currentUser = currentUser;
        _publisher = publisher;
        _mediator = mediator;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var dbContext = eventData.Context;

        if (dbContext is IIPDbContext iIPDbContext && iIPDbContext.EnableAuditing)
        {
            UpdateEntities(dbContext);
            PublishAuditTrailsAsync(eventData, iIPDbContext.Module).ConfigureAwait(false);
            PublishFileEntryAsync(eventData).ConfigureAwait(false);
        }

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext is IIPDbContext iIPDbContext && iIPDbContext.EnableAuditing)
        {
            UpdateEntities(dbContext);
            await PublishAuditTrailsAsync(eventData, iIPDbContext.Module);
            await PublishFileEntryAsync(eventData);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? dbContext)
    {
        if (dbContext == null)
            return;

        string? userName = _currentUser?.Name ?? "System";
        var userId = _currentUser.GetUserId();

        foreach (var entry in dbContext.ChangeTracker.Entries<IAuditableEntity>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedByName = userName;

                    // entry.Entity.LastModifiedBy = userId;
                    // entry.Entity.LastModifiedByName = userName;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedOn = SystemTime.Now();
                    entry.Entity.LastModifiedBy = userId;
                    entry.Entity.LastModifiedByName = userName;
                    break;

                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDelete)
                    {
                        softDelete.DeletedBy = userId;
                        softDelete.DeletedOn = SystemTime.Now();
                        softDelete.DeletedByName = userName;
                        entry.State = EntityState.Modified;
                    }

                    break;
            }
        }
    }

    private async Task PublishAuditTrailsAsync(DbContextEventData eventData, Domain.Enums.ApplicationModule module)
    {
        if (eventData.Context == null)
            return;
        eventData.Context.ChangeTracker.DetectChanges();

        var trails = new List<TrailDto>();
        var utcNow = SystemTime.Now();
        foreach (var entry in eventData.Context.ChangeTracker.Entries<IAggregateRoot>().Where(x => x.State is EntityState.Added or EntityState.Deleted or EntityState.Modified).ToList())
        {
            var trail = new TrailDto()
            {
                Id = Guid.NewGuid(),
                TableName = entry.Entity.GetType().Name,
                UserId = _currentUser.GetUserId(),
                DateTime = utcNow,
                Module = module

            };

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    continue;
                }
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    trail.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                var orginalValue = property.OriginalValue;
                var currentValue = property.CurrentValue;
                if (property.CurrentValue is DateOnly dateOnlyValue1)
                {
                    currentValue = dateOnlyValue1.ToDateTime(TimeOnly.MinValue);
                }

                if (property.OriginalValue is DateOnly dateOnlyValue2)
                {
                    orginalValue = dateOnlyValue2.ToDateTime(TimeOnly.MinValue);
                }

                if (property.OriginalValue is TimeOnly timeOnlyValue1)
                {
                    orginalValue = DateTime.MinValue.AddTicks(timeOnlyValue1.Ticks);
                }

                if (property.CurrentValue is TimeOnly timeOnlyValue2)
                {
                    currentValue = DateTime.MinValue.AddTicks(timeOnlyValue2.Ticks);
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        trail.Type = TrailType.Create;
                        trail.NewValues[propertyName] = currentValue;
                        break;

                    case EntityState.Deleted:
                        trail.Type = TrailType.Delete;
                        trail.OldValues[propertyName] = orginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified && property.OriginalValue == null && property.CurrentValue != null)
                        {
                            trail.ModifiedProperties.Add(propertyName);
                            trail.Type = TrailType.Delete;
                            trail.OldValues[propertyName] = orginalValue;
                            trail.NewValues[propertyName] = currentValue;
                        }
                        else if (property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false)
                        {
                            trail.ModifiedProperties.Add(propertyName);
                            trail.Type = TrailType.Update;
                            trail.OldValues[propertyName] = orginalValue;
                            trail.NewValues[propertyName] = currentValue;
                        }
                        break;
                }
            }

            trails.Add(trail);
        }
        if (trails.Count == 0)
            return;
        var auditTrails = new Collection<AuditTrail>();
        foreach (var trail in trails)
        {
            auditTrails.Add(trail.ToAuditTrail());
        }

        await _publisher.Publish(new AuditPublishedEvent(auditTrails));
    }

    private async Task PublishFileEntryAsync(DbContextEventData eventData)
    {
        if (eventData.Context == null)
            return;
        eventData.Context.ChangeTracker.DetectChanges();

        var fileEntries = new List<FileEntry>();
        foreach (var entry in eventData.Context.ChangeTracker.Entries<IEntity>().Select(e => e.Entity))
        {
            fileEntries.AddRange(entry.FileAttachments.Cast<FileEntry>());
            entry.ClearFileAttachments();
        }

        if (fileEntries.Count == 0)
            return;

        // await _publisher.Publish(new FileAttachmentPublishedEvent(fileEntries));
    }
}
