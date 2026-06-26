using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using Ardalis.Specification;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

internal static class ClientAccountActivityLogHelper
{
    public static string GetActionLabel(ClientAccountActivityAction action) => action switch
    {
        ClientAccountActivityAction.DeliveryMoved => "Delivery Moved",
        ClientAccountActivityAction.MealPlanChanged => "Meal Plan Changed",
        ClientAccountActivityAction.AdditionalItemAdded => "Additional Item Added",
        ClientAccountActivityAction.DeliveryCancelled => "Delivery Cancelled",
        ClientAccountActivityAction.DeliveryMethodChanged => "Delivery Method Changed",
        ClientAccountActivityAction.MealsReplaced => "Meals Replaced",
        ClientAccountActivityAction.ProfileUpdated => "Profile Updated",
        ClientAccountActivityAction.CutoffSettingsChanged => "Cutoff Settings Changed",
        ClientAccountActivityAction.ClientBlocked => "Client Blocked",
        ClientAccountActivityAction.ClientUnblocked => "Client Unblocked",
        _ => action.ToString()
    };

    public static ClientAccountActivityLogDto MapToDto(ClientAccountActivityLog entity)
    {
        var created = entity.CreatedOn;
        return new ClientAccountActivityLogDto
        {
            Id = entity.Id,
            Action = GetActionLabel(entity.Action),
            ActionType = entity.Action,
            PerformedBy = entity.PerformedByLabel ?? "System",
            PreviousValue = entity.PreviousValue,
            NewValue = entity.NewValue,
            CreatedOn = created,
            Date = DateOnly.FromDateTime(created),
            Time = TimeOnly.FromDateTime(created)
        };
    }

    public static IReadOnlyList<ClientAccountActivityAction> MatchActionLabels(string actionFilter)
    {
        var term = actionFilter.Trim().ToLowerInvariant();
        return Enum.GetValues<ClientAccountActivityAction>()
            .Where(a => GetActionLabel(a).ToLowerInvariant().Contains(term))
            .ToList();
    }

    public static void ApplyFilters(
        ISpecificationBuilder<ClientAccountActivityLog> query,
        DefaultIdType clientId,
        ClientAccountActivityLogFilterDto filters)
    {
        query.Where(x => x.ClientId == clientId);

        if (filters.ActionType.HasValue)
        {
            query.Where(x => x.Action == filters.ActionType.Value);
        }
        else if (!string.IsNullOrWhiteSpace(filters.Action))
        {
            var matchedActions = MatchActionLabels(filters.Action);
            if (matchedActions.Count == 0)
            {
                query.Where(_ => false);
                return;
            }

            query.Where(x => matchedActions.Contains(x.Action));
        }

        if (!string.IsNullOrWhiteSpace(filters.PreviousValue))
        {
            var previous = filters.PreviousValue.Trim().ToLowerInvariant();
            query.Where(x => x.PreviousValue != null && x.PreviousValue.ToLower().Contains(previous));
        }

        if (!string.IsNullOrWhiteSpace(filters.NewValue))
        {
            var next = filters.NewValue.Trim().ToLowerInvariant();
            query.Where(x => x.NewValue != null && x.NewValue.ToLower().Contains(next));
        }

        if (filters.Date.HasValue)
        {
            var dayStart = filters.Date.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var dayEnd = dayStart.AddDays(1);
            query.Where(x => x.CreatedOn >= dayStart && x.CreatedOn < dayEnd);
        }

        if (filters.Time.HasValue)
        {
            var time = filters.Time.Value;
            query.Where(x => x.CreatedOn.Hour == time.Hour && x.CreatedOn.Minute == time.Minute);
        }

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var term = filters.Search.Trim().ToLowerInvariant();
            var matchedActions = MatchActionLabels(term);
            query.Where(x =>
                (x.PreviousValue != null && x.PreviousValue.ToLower().Contains(term)) ||
                (x.NewValue != null && x.NewValue.ToLower().Contains(term)) ||
                (x.PerformedByLabel != null && x.PerformedByLabel.ToLower().Contains(term)) ||
                matchedActions.Contains(x.Action));
        }
    }

    public static async Task LogAsync(
        IRepository<ClientAccountActivityLog> repository,
        ICurrentUser currentUser,
        DefaultIdType clientId,
        ClientAccountActivityAction action,
        string? previousValue,
        string? newValue,
        DateOnly? relatedDate = null,
        ClientAccountActivitySource source = ClientAccountActivitySource.Admin,
        CancellationToken cancellationToken = default)
    {
        var performedByLabel = source switch
        {
            ClientAccountActivitySource.System => "System",
            ClientAccountActivitySource.Client => $"Client - {currentUser.Name ?? "Unknown"}",
            _ => $"Admin - {currentUser.Name ?? "Unknown"}"
        };

        await repository.AddAsync(new ClientAccountActivityLog
        {
            ClientId = clientId,
            Action = action,
            Source = source,
            PerformedByLabel = performedByLabel,
            PerformedByUserId = source == ClientAccountActivitySource.Admin ? currentUser.GetUserId() : null,
            PreviousValue = previousValue,
            NewValue = newValue,
            RelatedDate = relatedDate
        }, cancellationToken);
    }
}
