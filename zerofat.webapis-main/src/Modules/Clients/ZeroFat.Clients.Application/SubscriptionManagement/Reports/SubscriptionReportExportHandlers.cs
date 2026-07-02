using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Reports;

public class ExportNewSubscriptionsReportRequest : IQuery<Result<SubscriptionReportExportDto>>
{
    public DefaultIdType? ClientId { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public SubscriptionStatus? SubscriptionStatus { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }

    internal SubscriptionReportFilter ToFilter() => new()
    {
        ClientId = ClientId,
        PaymentStatus = PaymentStatus,
        SubscriptionStatus = SubscriptionStatus,
        FromDate = FromDate,
        ToDate = ToDate
    };
}

public class ExportRenewSubscriptionsReportRequest : IQuery<Result<SubscriptionReportExportDto>>
{
    public DefaultIdType? ClientId { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public SubscriptionStatus? SubscriptionStatus { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }

    internal SubscriptionReportFilter ToFilter() => new()
    {
        ClientId = ClientId,
        PaymentStatus = PaymentStatus,
        SubscriptionStatus = SubscriptionStatus,
        FromDate = FromDate,
        ToDate = ToDate
    };
}

public class ExportNewSubscriptionsReportRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<ClientSubscription> repository,
    IStringLocalizer<ExportNewSubscriptionsReportRequestHandler> localizer)
    : IQueryHandler<ExportNewSubscriptionsReportRequest, Result<SubscriptionReportExportDto>>
{
    public Task<Result<SubscriptionReportExportDto>> Handle(
        ExportNewSubscriptionsReportRequest request,
        CancellationToken cancellationToken)
        => SubscriptionReportExporter.ExportAsync(
            repository,
            currentUser,
            localizer,
            request.ToFilter(),
            isRenewReport: false,
            filePrefix: "new-subscriptions",
            sheetName: "New Subscriptions",
            cancellationToken);
}

public class ExportRenewSubscriptionsReportRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<ClientSubscription> repository,
    IStringLocalizer<ExportRenewSubscriptionsReportRequestHandler> localizer)
    : IQueryHandler<ExportRenewSubscriptionsReportRequest, Result<SubscriptionReportExportDto>>
{
    public Task<Result<SubscriptionReportExportDto>> Handle(
        ExportRenewSubscriptionsReportRequest request,
        CancellationToken cancellationToken)
        => SubscriptionReportExporter.ExportAsync(
            repository,
            currentUser,
            localizer,
            request.ToFilter(),
            isRenewReport: true,
            filePrefix: "renew-subscriptions",
            sheetName: "Renew Subscriptions",
            cancellationToken);
}

internal static class SubscriptionReportExporter
{
    public static async Task<Result<SubscriptionReportExportDto>> ExportAsync(
        IReadRepository<ClientSubscription> repository,
        ICurrentUser currentUser,
        IStringLocalizer localizer,
        SubscriptionReportFilter filter,
        bool isRenewReport,
        string filePrefix,
        string sheetName,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);

        var rows = await repository.ListAsync(
            new ClientSubscriptionsReportExportSpec(filter, isRenewReport),
            cancellationToken);

        var content = SubscriptionReportExcelBuilder.Build(sheetName, rows);
        var fromPart = filter.FromDate?.ToString("yyyyMMdd") ?? "all";
        var toPart = filter.ToDate?.ToString("yyyyMMdd") ?? "all";

        return await Result<SubscriptionReportExportDto>.SuccessAsync(new SubscriptionReportExportDto
        {
            FileName = $"{filePrefix}-{fromPart}-to-{toPart}.xlsx",
            Content = content,
            RowCount = rows.Count
        });
    }
}
