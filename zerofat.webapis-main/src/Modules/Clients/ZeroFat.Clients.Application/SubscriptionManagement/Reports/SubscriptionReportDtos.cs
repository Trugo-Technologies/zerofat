using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Reports;

public class SubscriptionReportFilter
{
    public DefaultIdType? ClientId { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public SubscriptionStatus? SubscriptionStatus { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
}

public class SubscriptionReportExportDto : IDto
{
    public string FileName { get; set; } = "subscriptions.xlsx";
    public byte[] Content { get; set; } = [];
    public int RowCount { get; set; }
}
