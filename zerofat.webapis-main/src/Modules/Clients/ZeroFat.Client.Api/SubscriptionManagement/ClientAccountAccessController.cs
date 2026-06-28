using System.Text;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;

namespace ZeroFat.ClientPortal.Api.SubscriptionManagement;

/// <summary>
/// Admin profile overview APIs for Manage Subscription UI (Screen 1).
/// Base route: /api/clientPortal-module/ClientAccountAccess
/// Auth: Bearer JWT — Admin role required.
///
///   GET {clientId}                      → full profile + subscription card + health metrics
///   PUT {clientId}                      → edit client profile + delivery addresses (admin)
///   GET {clientId}/subscription-summary → active plan, dates, remainingDays, totalDeliveredMeals
///   GET {clientId}/delivery-calendar → monthly calendar + summary counts
///   GET {clientId}/delivery-calendar/{date} → day detail drawer
///   GET/PUT {clientId}/delivery-calendar/cutoff-settings
///   PUT {clientId}/delivery-calendar/change-method
///   PUT {clientId}/delivery-calendar/cancel
///   PUT {clientId}/delivery-calendar/move
///   PUT {clientId}/delivery-calendar/add-items
///   PUT {clientId}/delivery-calendar/change-meal-plan
///   PUT {clientId}/delivery-calendar/replace-meals
///   GET {clientId}/delivery-calendar/add-on-options
///   POST {clientId}/delivery-calendar/add-items/preview
///   POST {clientId}/delivery-calendar/replace-meals/preview
///   POST {clientId}/delivery-calendar/change-meal-plan/preview
///   GET {clientId}/activity-logs
///   GET {clientId}/activity-logs/export
/// </summary>
internal sealed class ClientAccountAccessController(IClientPortalModule clientPortalModule) : BaseController
{
    /// <summary>Aggregated client account view for profile / manage-subscription entry.</summary>
    [HttpGet("{clientId:guid}")]
    public Task<Result<ClientAccountAccessDto>> GetAsync(DefaultIdType clientId)
        => clientPortalModule.ExecuteQueryAsync(new GetClientAccountAccessRequest(clientId));

    /// <summary>Update client personal info and delivery addresses from the account-access edit modal.</summary>
    [HttpPut("{clientId:guid}")]
    public Task<Result<ClientAccountAccessDto>> UpdateAsync(DefaultIdType clientId, UpdateClientAccountAccessRequest request)
    {
        request.ClientId = clientId;
        return clientPortalModule.ExecuteCommandAsync(request);
    }

    /// <summary>Block client app access (1 day, custom date, or permanent).</summary>
    [HttpPost("{clientId:guid}/block")]
    public Task<Result<ClientAccessControlDto>> BlockAsync(DefaultIdType clientId, BlockClientRequest request)
    {
        request.ClientId = clientId;
        return clientPortalModule.ExecuteCommandAsync(request);
    }

    /// <summary>Restore client app access after a block.</summary>
    [HttpPost("{clientId:guid}/unblock")]
    public Task<Result<ClientAccessControlDto>> UnblockAsync(DefaultIdType clientId)
        => clientPortalModule.ExecuteCommandAsync(new UnblockClientRequest(clientId));

    /// <summary>Subscription summary only — remaining days, delivered meal count, plan name.</summary>
    [HttpGet("{clientId:guid}/subscription-summary")]
    public Task<Result<ClientSubscriptionSummaryDto>> GetSubscriptionSummaryAsync(DefaultIdType clientId)
        => clientPortalModule.ExecuteQueryAsync(new GetClientSubscriptionSummaryRequest(clientId));

    /// <summary>Monthly delivery calendar for Client Account Access tab.</summary>
    [HttpGet("{clientId:guid}/delivery-calendar")]
    public Task<Result<ClientDeliveryCalendarResultDto>> GetDeliveryCalendarAsync(
        DefaultIdType clientId,
        [FromQuery] int month,
        [FromQuery] int year,
        [FromQuery] ClientDeliveryCalendarDayStatus? status)
        => clientPortalModule.ExecuteQueryAsync(new GetClientDeliveryCalendarRequest
        {
            ClientId = clientId,
            Month = month,
            Year = year,
            Status = status
        });

    /// <summary>Single-day delivery detail for calendar drawer.</summary>
    [HttpGet("{clientId:guid}/delivery-calendar/{date}")]
    public Task<Result<ClientDeliveryDayDetailDto>> GetDeliveryDayDetailAsync(DefaultIdType clientId, DateOnly date)
        => clientPortalModule.ExecuteQueryAsync(new GetClientDeliveryDayDetailRequest(clientId, date));

    [HttpGet("{clientId:guid}/delivery-calendar/cutoff-settings")]
    public Task<Result<ClientDeliveryCutoffSettingsDto>> GetCutoffSettingsAsync(DefaultIdType clientId)
        => clientPortalModule.ExecuteQueryAsync(new GetClientDeliveryCutoffSettingsRequest(clientId));

    [HttpPut("{clientId:guid}/delivery-calendar/cutoff-settings")]
    public Task<Result<ClientDeliveryCutoffSettingsDto>> UpdateCutoffSettingsAsync(
        DefaultIdType clientId,
        UpdateClientDeliveryCutoffSettingsRequest request)
    {
        request.ClientId = clientId;
        return clientPortalModule.ExecuteCommandAsync(request);
    }

    [HttpPut("{clientId:guid}/delivery-calendar/change-method")]
    public Task<Result<ClientDeliveryDayDetailDto>> ChangeDeliveryMethodAsync(
        DefaultIdType clientId,
        ChangeClientDeliveryMethodRequest request)
    {
        request.ClientId = clientId;
        return clientPortalModule.ExecuteCommandAsync(request);
    }

    [HttpPut("{clientId:guid}/delivery-calendar/cancel")]
    public Task<Result<ClientDeliveryDayDetailDto>> CancelDeliveryAsync(
        DefaultIdType clientId,
        CancelClientDeliveryRequest request)
    {
        request.ClientId = clientId;
        return clientPortalModule.ExecuteCommandAsync(request);
    }

    [HttpPut("{clientId:guid}/delivery-calendar/move")]
    public Task<Result<ClientDeliveryDayDetailDto>> MoveDeliveryAsync(
        DefaultIdType clientId,
        MoveClientDeliveryRequest request)
    {
        request.ClientId = clientId;
        return clientPortalModule.ExecuteCommandAsync(request);
    }

    [HttpPut("{clientId:guid}/delivery-calendar/add-items")]
    public Task<Result<ClientDeliveryDayDetailDto>> AddDeliveryItemsAsync(
        DefaultIdType clientId,
        AddClientDeliveryItemsRequest request)
    {
        request.ClientId = clientId;
        return clientPortalModule.ExecuteCommandAsync(request);
    }

    [HttpPut("{clientId:guid}/delivery-calendar/change-meal-plan")]
    public Task<Result<ClientDeliveryDayDetailDto>> ChangeDeliveryMealPlanAsync(
        DefaultIdType clientId,
        ChangeClientDeliveryMealPlanRequest request)
    {
        request.ClientId = clientId;
        return clientPortalModule.ExecuteCommandAsync(request);
    }

    [HttpPut("{clientId:guid}/delivery-calendar/replace-meals")]
    public Task<Result<ClientDeliveryDayDetailDto>> ReplaceDeliveryMealsAsync(
        DefaultIdType clientId,
        ReplaceClientDeliveryMealsRequest request)
    {
        request.ClientId = clientId;
        return clientPortalModule.ExecuteCommandAsync(request);
    }

    /// <summary>Add-on catalog for Additional Items modal (reuses wizard add-on options).</summary>
    [HttpGet("{clientId:guid}/delivery-calendar/add-on-options")]
    public Task<Result<List<WizardAddOnOptionDto>>> GetAddOnOptionsAsync(
        DefaultIdType clientId,
        [FromQuery] SubscriptionAddOnCategory? category)
        => clientPortalModule.ExecuteQueryAsync(new GetWizardAddOnsRequest(category));

    [HttpPost("{clientId:guid}/delivery-calendar/add-items/preview")]
    public Task<Result<ClientDeliveryAddOnPreviewDto>> PreviewAddItemsAsync(
        DefaultIdType clientId,
        PreviewClientDeliveryAddItemsRequest request)
    {
        request.ClientId = clientId;
        return clientPortalModule.ExecuteQueryAsync(request);
    }

    [HttpPost("{clientId:guid}/delivery-calendar/replace-meals/preview")]
    public Task<Result<ClientDeliveryReplaceMealsPreviewDto>> PreviewReplaceMealsAsync(
        DefaultIdType clientId,
        PreviewClientDeliveryReplaceMealsRequest request)
    {
        request.ClientId = clientId;
        return clientPortalModule.ExecuteQueryAsync(request);
    }

    [HttpPost("{clientId:guid}/delivery-calendar/change-meal-plan/preview")]
    public Task<Result<ClientDeliveryMealPlanChangePreviewDto>> PreviewChangeMealPlanAsync(
        DefaultIdType clientId,
        PreviewClientDeliveryMealPlanChangeRequest request)
    {
        request.ClientId = clientId;
        return clientPortalModule.ExecuteQueryAsync(request);
    }

    /// <summary>
    /// Paginated activity &amp; audit logs. All filters are optional — omit to return all records for the client.
    /// Query: search, action, actionType, previousValue, newValue, date, time, pageNumber, pageSize
    /// </summary>
    [HttpGet("{clientId:guid}/activity-logs")]
    public Task<PaginationResponse<ClientAccountActivityLogDto>> SearchActivityLogsAsync(
        DefaultIdType clientId,
        [FromQuery] string? search,
        [FromQuery] string? action,
        [FromQuery] ClientAccountActivityAction? actionType,
        [FromQuery] string? previousValue,
        [FromQuery] string? newValue,
        [FromQuery] DateOnly? date,
        [FromQuery] TimeOnly? time,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        => clientPortalModule.ExecuteQueryAsync(new SearchClientAccountActivityLogsRequest
        {
            ClientId = clientId,
            Search = search,
            Action = action,
            ActionType = actionType,
            PreviousValue = previousValue,
            NewValue = newValue,
            Date = date,
            Time = time,
            PageNumber = pageNumber,
            PageSize = pageSize
        });

    /// <summary>Export activity logs as CSV (supports same filters as GET activity-logs).</summary>
    [HttpGet("{clientId:guid}/activity-logs/export")]
    public async Task<IActionResult> ExportActivityLogsAsync(
        DefaultIdType clientId,
        [FromQuery] string? search,
        [FromQuery] string? action,
        [FromQuery] ClientAccountActivityAction? actionType,
        [FromQuery] string? previousValue,
        [FromQuery] string? newValue,
        [FromQuery] DateOnly? date,
        [FromQuery] TimeOnly? time)
    {
        var request = new ExportClientAccountActivityLogsRequest
        {
            ClientId = clientId,
            Search = search,
            Action = action,
            ActionType = actionType,
            PreviousValue = previousValue,
            NewValue = newValue,
            Date = date,
            Time = time
        };
        var result = await clientPortalModule.ExecuteQueryAsync(request);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(result);
        }

        var bytes = Encoding.UTF8.GetBytes(result.Data.Content);
        return File(bytes, "text/csv", result.Data.FileName);
    }
}
