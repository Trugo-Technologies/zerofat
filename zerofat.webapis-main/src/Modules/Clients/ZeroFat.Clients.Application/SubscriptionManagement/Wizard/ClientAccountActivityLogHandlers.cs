using System.Text;
using Ardalis.Specification;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

public class SearchClientAccountActivityLogsRequest : PaginationFilter, IQuery<PaginationResponse<ClientAccountActivityLogDto>>
{
    public DefaultIdType ClientId { get; set; }
    public string? Search { get; set; }
    public string? Action { get; set; }
    public ClientAccountActivityAction? ActionType { get; set; }
    public string? PreviousValue { get; set; }
    public string? NewValue { get; set; }
    public DateOnly? Date { get; set; }
    public TimeOnly? Time { get; set; }

    internal ClientAccountActivityLogFilterDto ToFilters() => new()
    {
        Search = Search,
        Action = Action,
        ActionType = ActionType,
        PreviousValue = PreviousValue,
        NewValue = NewValue,
        Date = Date,
        Time = Time
    };
}

public class ClientAccountActivityLogsBySearchSpec : Specification<ClientAccountActivityLog>
{
    public ClientAccountActivityLogsBySearchSpec(SearchClientAccountActivityLogsRequest request)
    {
        ClientAccountActivityLogHelper.ApplyFilters(Query, request.ClientId, request.ToFilters());
        Query.OrderByDescending(x => x.CreatedOn, !request.HasOrderBy())
            .PaginateBy(request);
    }
}

public class ClientAccountActivityLogsExportSpec : Specification<ClientAccountActivityLog>
{
    private const int MaxExportRows = 10_000;

    public ClientAccountActivityLogsExportSpec(ExportClientAccountActivityLogsRequest request)
    {
        ClientAccountActivityLogHelper.ApplyFilters(Query, request.ClientId, request.ToFilters());
        Query.OrderByDescending(x => x.CreatedOn).Take(MaxExportRows);
    }
}

public class SearchClientAccountActivityLogsRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<ClientAccountActivityLog> repository,
    IStringLocalizer<SearchClientAccountActivityLogsRequestHandler> localizer)
    : IQueryHandler<SearchClientAccountActivityLogsRequest, PaginationResponse<ClientAccountActivityLogDto>>
{
    public async Task<PaginationResponse<ClientAccountActivityLogDto>> Handle(
        SearchClientAccountActivityLogsRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);

        if (request.PageSize <= 0 || request.PageSize == int.MaxValue)
        {
            request.PageSize = 10;
        }

        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var spec = new ClientAccountActivityLogsBySearchSpec(request);
        var count = await repository.CountAsync(spec, cancellationToken);
        var items = await repository.ListAsync(spec, cancellationToken);

        return new PaginationResponse<ClientAccountActivityLogDto>(
            items.Select(ClientAccountActivityLogHelper.MapToDto).ToList(),
            count,
            pageNumber,
            request.PageSize);
    }
}

public class ExportClientAccountActivityLogsRequest : PaginationFilter, IQuery<Result<ClientAccountActivityLogExportDto>>
{
    public DefaultIdType ClientId { get; set; }
    public string? Search { get; set; }
    public string? Action { get; set; }
    public ClientAccountActivityAction? ActionType { get; set; }
    public string? PreviousValue { get; set; }
    public string? NewValue { get; set; }
    public DateOnly? Date { get; set; }
    public TimeOnly? Time { get; set; }

    internal ClientAccountActivityLogFilterDto ToFilters() => new()
    {
        Search = Search,
        Action = Action,
        ActionType = ActionType,
        PreviousValue = PreviousValue,
        NewValue = NewValue,
        Date = Date,
        Time = Time
    };
}

public class ExportClientAccountActivityLogsRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<ClientAccountActivityLog> repository,
    IStringLocalizer<ExportClientAccountActivityLogsRequestHandler> localizer)
    : IQueryHandler<ExportClientAccountActivityLogsRequest, Result<ClientAccountActivityLogExportDto>>
{
    public async Task<Result<ClientAccountActivityLogExportDto>> Handle(
        ExportClientAccountActivityLogsRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);

        var rows = await repository.ListAsync(
            new ClientAccountActivityLogsExportSpec(request),
            cancellationToken);

        var builder = new StringBuilder();
        builder.AppendLine("Action,Performed By,Previous Value,New Value,Date,Time");

        foreach (var row in rows)
        {
            var dto = ClientAccountActivityLogHelper.MapToDto(row);
            builder.AppendLine(string.Join(",",
                CsvEscape(dto.Action),
                CsvEscape(dto.PerformedBy),
                CsvEscape(dto.PreviousValue ?? "—"),
                CsvEscape(dto.NewValue ?? "—"),
                dto.Date.ToString("yyyy-MM-dd"),
                dto.Time.ToString("HH:mm")));
        }

        return await Result<ClientAccountActivityLogExportDto>.SuccessAsync(new ClientAccountActivityLogExportDto
        {
            FileName = $"client-{request.ClientId}-activity-logs.csv",
            Content = builder.ToString(),
            RowCount = rows.Count
        });
    }

    private static string CsvEscape(string value)
    {
        if (value.Contains('"') || value.Contains(',') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
        }

        return value;
    }
}
