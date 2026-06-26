using ZeroFat.Domain.Common.Contracts;

namespace ZeroFat.ClientPortal.Domain.SubscriptionManagement;

public enum ClientAccountActivityAction
{
    DeliveryMoved,
    MealPlanChanged,
    AdditionalItemAdded,
    DeliveryCancelled,
    DeliveryMethodChanged,
    MealsReplaced,
    ProfileUpdated,
    CutoffSettingsChanged,
    ClientBlocked,
    ClientUnblocked
}

public enum ClientAccountActivitySource
{
    Admin,
    System,
    Client
}

/// <summary>
/// Audit trail for Client Account Access admin tab (Activity &amp; Audit Logs).
/// </summary>
public class ClientAccountActivityLog : AuditableEntity, IAggregateRoot
{
    public DefaultIdType ClientId { get; set; }
    public ClientAccountActivityAction Action { get; set; }
    public ClientAccountActivitySource Source { get; set; }
    public string? PerformedByLabel { get; set; }
    public DefaultIdType? PerformedByUserId { get; set; }
    public string? PreviousValue { get; set; }
    public string? NewValue { get; set; }
    public DateOnly? RelatedDate { get; set; }
}
