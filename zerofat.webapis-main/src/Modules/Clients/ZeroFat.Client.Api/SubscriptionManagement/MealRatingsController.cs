using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.MealRatings;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;

namespace ZeroFat.ClientPortal.Api.SubscriptionManagement;

/// <summary>
/// Meal rating APIs for mobile app and admin panel.
/// Base route: /api/clientPortal-module/MealRatings
///
///   POST submit                              → client submits/updates meal feedback
///   GET  by-daily-meal/{dailyMealSelectionId} → get feedback for a meal card
///   POST search                              → admin paginated list
///   PUT  {id}/reply                          → admin reply to feedback
/// </summary>
internal sealed class MealRatingsController(IClientPortalModule clientPortalModule) : BaseController
{
    /// <summary>Submit or update feedback for a delivered meal (Not good / Okay / Awesome).</summary>
    [HttpPost("submit")]
    public Task<Result<MealRatingSummaryDto>> SubmitAsync(SubmitMealRatingRequest request)
        => clientPortalModule.ExecuteCommandAsync(request);

    /// <summary>Get existing feedback for a daily meal selection (shown on meal card).</summary>
    [HttpGet("by-daily-meal/{dailyMealSelectionId:guid}")]
    public Task<Result<MealRatingSummaryDto>> GetByDailyMealAsync(DefaultIdType dailyMealSelectionId)
        => clientPortalModule.ExecuteQueryAsync(new GetMealRatingRequest(dailyMealSelectionId));

    /// <summary>Admin meal ratings table with search and filters.</summary>
    [HttpPost("search")]
    public Task<PaginationResponse<MealRatingDto>> SearchAsync(SearchMealRatingsRequest request)
        => clientPortalModule.ExecuteQueryAsync(request);

    /// <summary>Admin reply to customer meal feedback.</summary>
    [HttpPut("{id:guid}/reply")]
    public Task<Result<MealRatingDto>> ReplyAsync(DefaultIdType id, ReplyMealRatingRequest request)
    {
        request.Id = id;
        return clientPortalModule.ExecuteCommandAsync(request);
    }
}
