using ZeroFat.Application.Common.Interfaces;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

public class ClientAccountActivityLogDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public ClientAccountActivityAction ActionType { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public string? PreviousValue { get; set; }
    public string? NewValue { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public DateTime CreatedOn { get; set; }
}

/// <summary>Optional filters for activity log list/export. Omit any field to skip that filter.</summary>
public class ClientAccountActivityLogFilterDto
{
    public string? Search { get; set; }
    public string? Action { get; set; }
    public ClientAccountActivityAction? ActionType { get; set; }
    public string? PreviousValue { get; set; }
    public string? NewValue { get; set; }
    public DateOnly? Date { get; set; }
    public TimeOnly? Time { get; set; }
}

public class ClientAccountActivityLogExportDto : IDto
{
    public string FileName { get; set; } = "activity-logs.csv";
    public string Content { get; set; } = string.Empty;
    public int RowCount { get; set; }
}
