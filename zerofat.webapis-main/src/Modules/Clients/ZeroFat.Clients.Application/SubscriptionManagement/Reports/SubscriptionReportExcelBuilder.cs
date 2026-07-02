using ClosedXML.Excel;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Reports;

internal static class SubscriptionReportExcelBuilder
{
    private static readonly string[] Headers =
    [
        "Subscription ID",
        "Client Name",
        "Client Email",
        "Client Mobile",
        "Meal Plan",
        "Subscription Type",
        "Subscription Status",
        "Payment Status",
        "Start Date",
        "End Date",
        "Total Cost (AED)",
        "VAT Amount (AED)",
        "Promo Code",
        "Renewal Count",
        "Auto Renewal",
        "Payment Date",
        "Created On",
        "Origin",
        "Plan Variant",
        "Calorie Target"
    ];

    public static byte[] Build(string sheetName, IReadOnlyList<ClientSubscription> subscriptions)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);

        for (var column = 0; column < Headers.Length; column++)
        {
            worksheet.Cell(1, column + 1).Value = Headers[column];
        }

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        var rowIndex = 2;
        foreach (var subscription in subscriptions)
        {
            worksheet.Cell(rowIndex, 1).Value = subscription.Id.ToString();
            worksheet.Cell(rowIndex, 2).Value = subscription.Client?.FullName ?? string.Empty;
            worksheet.Cell(rowIndex, 3).Value = subscription.Client?.Email ?? string.Empty;
            worksheet.Cell(rowIndex, 4).Value = subscription.Client?.Mobile ?? string.Empty;
            worksheet.Cell(rowIndex, 5).Value = subscription.MealPlan?.NameEn ?? string.Empty;
            worksheet.Cell(rowIndex, 6).Value = subscription.SubscriptionType.ToString();
            worksheet.Cell(rowIndex, 7).Value = subscription.SubscriptionStatus.ToString();
            worksheet.Cell(rowIndex, 8).Value = subscription.PaymentStatus.ToString();
            worksheet.Cell(rowIndex, 9).Value = subscription.StartDate.ToString("yyyy-MM-dd");
            worksheet.Cell(rowIndex, 10).Value = subscription.EndDate.ToString("yyyy-MM-dd");
            worksheet.Cell(rowIndex, 11).Value = subscription.TotalCost;
            worksheet.Cell(rowIndex, 12).Value = subscription.VatAmount ?? 0m;
            worksheet.Cell(rowIndex, 13).Value = subscription.PromoCode ?? string.Empty;
            worksheet.Cell(rowIndex, 14).Value = subscription.RenewalCount;
            worksheet.Cell(rowIndex, 15).Value = subscription.IsAutoRenewalEnabled ? "Yes" : "No";
            worksheet.Cell(rowIndex, 16).Value = subscription.PaymentDate?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty;
            worksheet.Cell(rowIndex, 17).Value = subscription.CreatedOn.ToString("yyyy-MM-dd HH:mm");
            worksheet.Cell(rowIndex, 18).Value = subscription.CreatedByAdminId.HasValue ? "Admin" : "Client";
            worksheet.Cell(rowIndex, 19).Value = subscription.PlanVariant ?? string.Empty;
            worksheet.Cell(rowIndex, 20).Value = subscription.CalorieTarget?.ToString() ?? string.Empty;
            rowIndex++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
