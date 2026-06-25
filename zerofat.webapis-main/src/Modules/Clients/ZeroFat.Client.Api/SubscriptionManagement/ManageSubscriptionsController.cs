using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;

namespace ZeroFat.ClientPortal.Api.SubscriptionManagement;

/// <summary>
/// Admin-only Manage Subscription wizard APIs (admin creates/renews subscription on behalf of a client).
/// Base route: /api/clientPortal-module/ManageSubscriptions
/// Auth: Bearer JWT — Admin role required (enforced in application handlers).
///
/// Typical flow:
///   POST   wizard/start                          → draftId
///   PUT    wizard/{draftId}/renewal-options      → renew only
///   PUT    wizard/{draftId}/configuration        → meal plan, meals, add-ons
///   PUT    wizard/{draftId}/schedule             → duration, calendar, address, discounts
///   GET    wizard/{draftId}/delivery-calendar    → ?month=6&amp;year=2026
///   GET    wizard/{draftId}/preview              → billing quote (no DB save)
///   PUT    wizard/{draftId}/billing              → customer email + optional note
///   POST   wizard/{draftId}/finalize             → ClientSubscription (Pending) + Stripe link + email job
///   POST   wizard/{draftId}/save-draft           → explicit save
///   DELETE wizard/{draftId}                      → cancel draft
/// </summary>
internal sealed class ManageSubscriptionsController(IClientPortalModule clientPortalModule) : BaseController
{
    /// <summary>Start wizard — body: { "clientId": "...", "wizardMode": "New" | "Renew" }.</summary>
    [HttpPost("wizard/start")]
    public Task<Result<SubscriptionWizardDraftDto>> StartWizardAsync(StartSubscriptionWizardRequest request)
        => clientPortalModule.ExecuteCommandAsync(request);

    /// <summary>Load draft state — GET wizard/{draftId}.</summary>
    [HttpGet("wizard/{draftId:guid}")]
    public Task<Result<SubscriptionWizardDraftDto>> GetWizardAsync(DefaultIdType draftId)
        => clientPortalModule.ExecuteQueryAsync(new GetSubscriptionWizardRequest(draftId));

    /// <summary>Renewal step — body: { "renewalStrategy": "ExtendAfterExpiry" | "ScheduleFutureDate", "scheduledStartDate": "2026-07-01" }.</summary>
    [HttpPut("wizard/{draftId:guid}/renewal-options")]
    public async Task<Result<SubscriptionWizardDraftDto>> UpdateRenewalOptionsAsync(
        DefaultIdType draftId,
        UpdateRenewalOptionsRequest request)
    {
        request.DraftId = draftId;
        return await clientPortalModule.ExecuteCommandAsync(request);
    }

    /// <summary>Configuration step — meal plan, variant, calories, protein, meal types, add-ons.</summary>
    [HttpPut("wizard/{draftId:guid}/configuration")]
    public async Task<Result<SubscriptionWizardDraftDto>> UpdateConfigurationAsync(
        DefaultIdType draftId,
        UpdateWizardConfigurationRequest request)
    {
        request.DraftId = draftId;
        return await clientPortalModule.ExecuteCommandAsync(request);
    }

    /// <summary>Schedule step — duration, skip weekends, delivery days/dates, address, promo, manual discount.</summary>
    [HttpPut("wizard/{draftId:guid}/schedule")]
    public async Task<Result<SubscriptionWizardDraftDto>> UpdateScheduleAsync(
        DefaultIdType draftId,
        UpdateWizardScheduleRequest request)
    {
        request.DraftId = draftId;
        return await clientPortalModule.ExecuteCommandAsync(request);
    }

    /// <summary>Billing step — customer email and optional message before finalize.</summary>
    [HttpPut("wizard/{draftId:guid}/billing")]
    public async Task<Result<SubscriptionWizardDraftDto>> UpdateBillingAsync(
        DefaultIdType draftId,
        UpdateWizardBillingRequest request)
    {
        request.DraftId = draftId;
        return await clientPortalModule.ExecuteCommandAsync(request);
    }

    /// <summary>Meal plan catalog for wizard — GET wizard/options/meal-plans?clientId={guid}.</summary>
    [HttpGet("wizard/options/meal-plans")]
    public Task<Result<List<WizardMealPlanOptionDto>>> GetMealPlansAsync([FromQuery] DefaultIdType clientId)
        => clientPortalModule.ExecuteQueryAsync(new GetWizardMealPlansRequest(clientId));

    /// <summary>Add-on catalog — GET wizard/options/add-ons?category=Meals|Snacks|Drinks|Supplements.</summary>
    [HttpGet("wizard/options/add-ons")]
    public Task<Result<List<WizardAddOnOptionDto>>> GetAddOnsAsync([FromQuery] SubscriptionAddOnCategory? category)
        => clientPortalModule.ExecuteQueryAsync(new GetWizardAddOnsRequest(category));

    /// <summary>Delivery calendar for schedule UI — GET wizard/{draftId}/delivery-calendar?month=6&amp;year=2026.</summary>
    [HttpGet("wizard/{draftId:guid}/delivery-calendar")]
    public async Task<Result<DeliveryCalendarResult>> GetDeliveryCalendarAsync(
        DefaultIdType draftId,
        [FromQuery] int month,
        [FromQuery] int year)
        => await clientPortalModule.ExecuteQueryAsync(new GetWizardDeliveryCalendarRequest
        {
            DraftId = draftId,
            Month = month,
            Year = year
        });

    /// <summary>Billing preview (plan + VAT + discounts) without persisting subscription.</summary>
    [HttpGet("wizard/{draftId:guid}/preview")]
    public Task<Result<SubscriptionWizardPreviewDto>> GetPreviewAsync(DefaultIdType draftId)
        => clientPortalModule.ExecuteQueryAsync(new GetWizardPreviewRequest(draftId));

    /// <summary>Explicit save — same effect as step PUTs; draft remains editable.</summary>
    [HttpPost("wizard/{draftId:guid}/save-draft")]
    public Task<Result<SubscriptionWizardDraftDto>> SaveDraftAsync(DefaultIdType draftId)
        => clientPortalModule.ExecuteCommandAsync(new SaveSubscriptionWizardDraftRequest(draftId));

    /// <summary>
    /// Create Pending ClientSubscription, Stripe payment link, enqueue payment-link email.
    /// Returns ClientSubscriptionSimplifyDto with paymentQuickLink.
    /// </summary>
    [HttpPost("wizard/{draftId:guid}/finalize")]
    public Task<Result<ClientSubscriptionSimplifyDto>> FinalizeAsync(DefaultIdType draftId)
        => clientPortalModule.ExecuteCommandAsync(new FinalizeSubscriptionWizardRequest(draftId));

    /// <summary>Cancel draft — DELETE wizard/{draftId}.</summary>
    [HttpDelete("wizard/{draftId:guid}")]
    public Task<Result> CancelWizardAsync(DefaultIdType draftId)
        => clientPortalModule.ExecuteCommandAsync(new CancelSubscriptionWizardRequest(draftId));
}
