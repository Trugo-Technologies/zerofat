using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.Reports;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Api.SubscriptionManagement;

/// <summary>
/// Admin subscription Excel reports.
/// Base route: /api/clientPortal-module/SubscriptionReports
/// Auth: Bearer JWT — Admin role required.
///
///   GET new-subscriptions/export
///   GET renew-subscriptions/export
/// </summary>
internal sealed class SubscriptionReportsController(IClientPortalModule clientPortalModule) : BaseController
{
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    /// <summary>Export new subscriptions (RenewalCount = 0) as Excel.</summary>
    [HttpGet("new-subscriptions/export")]
    public async Task<IActionResult> ExportNewSubscriptionsAsync(
        [FromQuery] DefaultIdType? clientId,
        [FromQuery] PaymentStatus? paymentStatus,
        [FromQuery] SubscriptionStatus? subscriptionStatus,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
    {
        var result = await clientPortalModule.ExecuteQueryAsync(new ExportNewSubscriptionsReportRequest
        {
            ClientId = clientId,
            PaymentStatus = paymentStatus,
            SubscriptionStatus = subscriptionStatus,
            FromDate = fromDate,
            ToDate = toDate
        });

        return ToExcelFileResult(result);
    }

    /// <summary>Export renew subscriptions (RenewalCount &gt; 0) as Excel.</summary>
    [HttpGet("renew-subscriptions/export")]
    public async Task<IActionResult> ExportRenewSubscriptionsAsync(
        [FromQuery] DefaultIdType? clientId,
        [FromQuery] PaymentStatus? paymentStatus,
        [FromQuery] SubscriptionStatus? subscriptionStatus,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
    {
        var result = await clientPortalModule.ExecuteQueryAsync(new ExportRenewSubscriptionsReportRequest
        {
            ClientId = clientId,
            PaymentStatus = paymentStatus,
            SubscriptionStatus = subscriptionStatus,
            FromDate = fromDate,
            ToDate = toDate
        });

        return ToExcelFileResult(result);
    }

    private static IActionResult ToExcelFileResult(Result<SubscriptionReportExportDto> result)
    {
        if (!result.Succeeded || result.Data == null)
        {
            return new BadRequestObjectResult(result);
        }

        return new FileContentResult(result.Data.Content, ExcelContentType)
        {
            FileDownloadName = result.Data.FileName
        };
    }
}
