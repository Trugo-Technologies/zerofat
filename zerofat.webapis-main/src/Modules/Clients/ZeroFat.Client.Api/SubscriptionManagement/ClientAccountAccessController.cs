using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

namespace ZeroFat.ClientPortal.Api.SubscriptionManagement;

/// <summary>
/// Admin profile overview APIs for Manage Subscription UI (Screen 1).
/// Base route: /api/clientPortal-module/ClientAccountAccess
/// Auth: Bearer JWT — Admin role required.
///
///   GET {clientId}                      → full profile + subscription card + health metrics
///   GET {clientId}/subscription-summary → active plan, dates, remainingDays, totalDeliveredMeals
/// </summary>
internal sealed class ClientAccountAccessController(IClientPortalModule clientPortalModule) : BaseController
{
    /// <summary>Aggregated client account view for profile / manage-subscription entry.</summary>
    [HttpGet("{clientId:guid}")]
    public Task<Result<ClientAccountAccessDto>> GetAsync(DefaultIdType clientId)
        => clientPortalModule.ExecuteQueryAsync(new GetClientAccountAccessRequest(clientId));

    /// <summary>Subscription summary only — remaining days, delivered meal count, plan name.</summary>
    [HttpGet("{clientId:guid}/subscription-summary")]
    public Task<Result<ClientSubscriptionSummaryDto>> GetSubscriptionSummaryAsync(DefaultIdType clientId)
        => clientPortalModule.ExecuteQueryAsync(new GetClientSubscriptionSummaryRequest(clientId));
}
