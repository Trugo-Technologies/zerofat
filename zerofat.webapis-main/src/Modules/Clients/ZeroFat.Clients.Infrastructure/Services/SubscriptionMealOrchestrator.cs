using System.Threading;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.DailySelections;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.ClientPortal.Infrastructure.Persistence.Context;
using ZeroFat.Domain.Common;
using ZeroFat.Domain.Enums;
using ZeroFat.Infrastructure.Api.Auth;
using ZeroFat.Infrastructure.Stripe;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.ClientPortal.Infrastructure.Services;

/// <summary>
/// Automatically assigns default meals to customer subscriptions when their selected meals are null.
/// This ensures customers have meals assigned even when menus are only published 2-3 weeks in advance,
/// while their subscriptions are for 3-month periods.
/// </summary>
public class SubscriptionMealOrchestrator : ISubscriptionMealOrchestrator
{
    private readonly ClientPortalContext _dbContext;
    private readonly IStripeService _stripeService;
    private readonly IClientPortalSettingservice _clientPortalSettingservice;
    private readonly IJobService _jobService;
    private readonly IRepository<DailyMenuMeal> _dailyMenuMealRepo;
    private readonly IRepository<Meal> _mealRepo;
    private readonly ILogger<SubscriptionMealOrchestrator> _logger;

    /// <summary>
    /// Initializes a new instance of the DefaultMealAssignmentService
    /// </summary>
    /// <param name="dbContext">Database context for client portal</param>
    /// <param name="dailyMenuMealRepo">Repository for accessing daily menu meals</param>
    public SubscriptionMealOrchestrator(
        ClientPortalContext dbContext,
        IStripeService stripeService,
        IClientPortalSettingservice clientPortalSettingservice,
        ILogger<SubscriptionMealOrchestrator> logger,
        IJobService jobService,
        IRepository<DailyMenuMeal> dailyMenuMealRepo,

        IRepository<Meal> mealRepo)
    {
        _dbContext = dbContext;
        _dailyMenuMealRepo = dailyMenuMealRepo;
        _mealRepo = mealRepo;
        _stripeService = stripeService;
        _logger = logger;
        _clientPortalSettingservice = clientPortalSettingservice;
        _jobService = jobService;
    }


    // <summary>
    /// Creates daily meal selections for a new subscription
    /// </summary>
    public async Task CreateDailySelectionsForSubscriptionAsync(Guid subscriptionId)
    {
        using var _ = _logger.BeginScope("Creating daily selections for subscription {SubscriptionId}", subscriptionId);

        try
        {
            var subscription = await _dbContext.ClientSubscriptions
                .Include(x => x.SelectedMealTypes)
                .FirstOrDefaultAsync(x => x.Id == subscriptionId);

            if (subscription == null)
            {
                _logger.LogWarning("Subscription {SubscriptionId} not found", subscriptionId);
                return;
            }

            var dailySelections = await BuildDailySelections(subscription);
            await _dbContext.DailySelections.AddRangeAsync(dailySelections);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation(message: "Created {Count} daily selections for subscription {SubscriptionId}",
                dailySelections.Count, subscriptionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create daily selections for subscription {SubscriptionId}", subscriptionId);
            throw;
        }
    }

    private async Task<List<DailySelection>> BuildDailySelections(ClientSubscription subscription)
    {
        var dailySelections = new List<DailySelection>();

        for (var date = subscription.StartDate; date <= subscription.EndDate; date = date.AddDays(1))
        {
            if (!subscription.SelectedDeliveryDays.Contains(date.DayOfWeek))
                continue;

            var dailySelection = await BuildDailySelection(subscription, date);
            dailySelections.Add(dailySelection);
        }

        return dailySelections;
    }

    private async Task<DailySelection> BuildDailySelection(ClientSubscription subscription, DateOnly date)
    {
        var dailySelection = new DailySelection
        {
            ClientId = subscription.ClientId,
            ClientSubscriptionId = subscription.Id,
            ClientLocationId = subscription.ClientLocationId ?? Guid.Empty,
            DeliveryTime = subscription.PreferredDeliveryTime,
            MealPlanId = subscription.MealPlanId,
            Date = date,
            DailySelectionStatus = DailySelectionStatus.Pending,
            HasCutlery = true,
        };

        foreach (var mealType in subscription.SelectedMealTypes)
        {
            await AddMealSelections(dailySelection, subscription, date, mealType);
        }

        return dailySelection;
    }

    private async Task AddMealSelections(DailySelection dailySelection,
        ClientSubscription subscription,
        DateOnly date,
        MealTypeSelection mealType)
    {
        var now = DateOnly.FromDateTime(DateTime.Now);

        for (int i = 0; i < mealType.QuantityPerDay; i++)
        {
            var meal = await _dailyMenuMealRepo.FirstOrDefaultAsync(
                new CurrentDailySelectionByIdSpec(
                    date,
                    mealType.MealTypeId,
                    subscription.MealPlanId));

            var mealSelection = new DailyMealSelection
            {
                ClientId = subscription.ClientId,
                ClientSubscriptionId = subscription.Id,
                MealTypeId = mealType.MealTypeId,
                MealPlanId = dailySelection.MealPlanId,
                Date = date,
                BasePrice = mealType.Price,
                IsPaid = true,
                MealSelectionType = MealSelectionType.Default,

            };

            if (meal?.Meal != null && date <= now.AddDays(21))
            {
                mealSelection.MealId = meal.Meal.Id;
                mealSelection.TotalCalories = meal.Meal.Calories;
                mealSelection.TotalProteins = meal.Meal.Protein;
                mealSelection.TotalFats = meal.Meal.Fat;
                mealSelection.TotalCarbohydrates = meal.Meal.Carbs;

                dailySelection.TotalCarbohydrates += meal.Meal.Carbs;
                dailySelection.TotalCalories += meal.Meal.Calories;
                dailySelection.TotalFats += meal.Meal.Fat;
                dailySelection.TotalProteins += meal.Meal.Protein;
            }

            dailySelection.DailyMealSelections.Add(mealSelection);
        }
    }

    /// <summary>
    /// Assigns default meals to all customer daily selections that have null meals.
    /// Updates nutritional totals for each daily selection after assignment.
    /// Processes in batches of 25 selections at a time.
    /// </summary>
    public async Task AssignDefaultMealsToSubscriptionsAsync()
    {
        const int batchSize = 25;
        using var loggerScope = _logger.BeginScope("Default Meal Assignment Job");

        var now = DateOnly.FromDateTime(DateTime.Now);
        try
        {
            _logger.LogInformation("Starting default meal assignment process");

            // Get base query for selections needing assignments
            var query = _dbContext.DailySelections
                .Include(x => x.Client)
                .Include(x => x.DailyMealSelections)
                .Where(x => x.DailyMealSelections.Any(m => m.MealId == null)
                         && x.Date >= now && x.Date <= now.AddDays(21))
                .OrderBy(x => x.Date); // Process oldest first

            int totalProcessed = 0;
            int totalAssigned = 0;
            int failedAssignments = 0;
            bool moreRecordsExist = true;

            while (moreRecordsExist)
            {
                // Get batch of 25 records
                var dailySelections = await query
                    .Skip(totalProcessed)
                    .Take(batchSize)
                    .ToListAsync();

                if (dailySelections.Count == 0)
                {
                    moreRecordsExist = false;
                    if (totalProcessed == 0)
                    {
                        _logger.LogInformation("No daily selections with unassigned meals found");
                    }
                    continue;
                }

                _logger.LogDebug("Processing batch of {BatchCount} selections (total processed: {TotalProcessed})",
                    dailySelections.Count, totalProcessed);

                foreach (var dailySelection in dailySelections)
                {
                    // Simple string message approach
                    using var selectionScope = _logger.BeginScope(
                        "Client:{ClientName} Selection:{DailySelectionId} Date:{Date}",
                        dailySelection.Client?.FullName ?? "Unknown",
                        dailySelection.Id,
                        dailySelection.Date.ToString("yyyy-MM-dd"));

                    try
                    {
                        var unassignedMeals = dailySelection.DailyMealSelections
                            .Where(x => x.MealId == null && x.MealPlanId != null && x.MealTypeId != null)
                            .ToList();

                        _logger.LogDebug("Found {UnassignedCount} unassigned meals", unassignedMeals.Count);

                        foreach (var unassignedMeal in unassignedMeals)
                        {
                            var success = await AssignDefaultMealAsync(dailySelection, unassignedMeal);
                            if (success) totalAssigned++;
                            else failedAssignments++;
                        }

                        totalProcessed++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Failed to process selection for {ClientName} (ID: {DailySelectionId})",
                            dailySelection.Client?.FullName ?? "Unknown Client",
                            dailySelection.Id);
                        failedAssignments++;
                    }
                }

                // Save after each batch
                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogDebug("Successfully saved batch {BatchNumber}",
                        (totalProcessed / batchSize) + 1);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save batch {BatchNumber}",
                        (totalProcessed / batchSize) + 1);
                    // Continue with next batch despite save failure
                }
            }

            _logger.LogInformation(
                "Completed meal assignment process. " +
                "Total processed: {TotalProcessed}, " +
                "Meals assigned: {AssignedCount}, " +
                "Failures: {FailedCount}",
                totalProcessed,
                totalAssigned,
                failedAssignments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical failure in default meal assignment job");
            throw;
        }
    }

    /// <summary>
    /// Assigns a default meal to a specific daily meal selection and updates nutritional totals
    /// </summary>
    /// <param name="dailySelection">The parent daily selection record</param>
    /// <param name="mealSelection">The specific meal selection to update</param>
    private async Task<bool> AssignDefaultMealAsync(DailySelection dailySelection, DailyMealSelection mealSelection)
    {
        try
        {
            var defaultMeal = await _dailyMenuMealRepo.FirstOrDefaultAsync(
                new CurrentDailySelectionByIdSpec(
                    mealSelection.Date,
                    mealSelection.MealTypeId!.Value,
                    mealSelection.MealPlanId!.Value));

            if (defaultMeal?.Meal != null)
            {
                // Update meal assignment and nutrition values
                mealSelection.MealId = defaultMeal.Meal.Id;
                mealSelection.TotalCalories = defaultMeal.Meal.Calories;
                mealSelection.TotalProteins = defaultMeal.Meal.Protein;
                mealSelection.TotalFats = defaultMeal.Meal.Fat;
                mealSelection.TotalCarbohydrates = defaultMeal.Meal.Carbs;

                // Update daily selection totals
                dailySelection.TotalCalories += defaultMeal.Meal!.Calories;
                dailySelection.TotalProteins += defaultMeal.Meal!.Protein;
                dailySelection.TotalFats += defaultMeal.Meal!.Fat;
                dailySelection.TotalCarbohydrates += defaultMeal.Meal!.Carbs;

                _logger.LogDebug(
                    "Assigned default meal {MealId} ({MealName}) to selection",
                    defaultMeal.Meal.Id,
                    defaultMeal.Meal.NameEn);

                return true;
            }

            _logger.LogWarning(
                "No default meal found for MealType {MealTypeId} on {Date}",
                mealSelection.MealTypeId,
                mealSelection.Date.ToString("yyyy-MM-dd"));

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to assign default meal for MealType {MealTypeId}",
                mealSelection.MealTypeId);
            return false;
        }
    }

    /// <summary>
    /// Creates daily meal selections for a client order by populating each day between
    /// the order's start and end dates with the selected meals and their nutritional values.
    /// </summary>
    /// <param name="orderId">The ID of the client order to process</param>
    /// <exception cref="ArgumentException">Thrown when the order is not found</exception>
    public async Task CreateDailyMealSelectionsForOrderAsync(Guid orderId)
    {
        using var loggerScope = _logger.BeginScope("Processing daily meal selections for order {OrderId}", orderId);

        try
        {
            // 1. Retrieve and validate the order
            var clientOrder = await _dbContext.ClientOrders
                .Include(o => o.ClientOrderItems)
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (clientOrder == null)
            {
                _logger.LogWarning("Client order {OrderId} not found", orderId);
                throw new ArgumentException($"Client order {orderId} not found");
            }

            _logger.LogInformation("Processing order from {StartDate} to {EndDate} with {ItemCount} meal types",
                clientOrder.StartDate, clientOrder.EndDate, clientOrder.ClientOrderItems.Count);

            // 2. Get existing daily selections for the date range
            var dateRangeQuery = _dbContext.DailySelections
                .Include(x => x.Client)
                .Where(x => x.Date >= clientOrder.StartDate
                         && x.Date <= clientOrder.EndDate)
                .Where(x => x.ClientId == clientOrder.ClientId)
                .OrderBy(x => x.Date);

            var dailySelections = await dateRangeQuery.ToListAsync();
            _logger.LogDebug("Found {SelectionCount} existing daily selections", dailySelections.Count);

            // 3. Get all relevant meal data in one query
            var mealIds = clientOrder.ClientOrderItems.Select(x => x.MealId).ToList();
            var meals = await _mealRepo.ListAsync(new ExpressionSpecification<Meal>(x => mealIds.Contains(x.Id)));
            _logger.LogDebug("Retrieved {MealCount} meal definitions", meals.Count);

            // 4. Process each day and create meal selections
            int createdSelections = 0;
            foreach (var dailySelection in dailySelections)
            {
                foreach (var orderItem in clientOrder.ClientOrderItems)
                {
                    var meal = meals.FirstOrDefault(x => x.Id == orderItem.MealId);
                    if (meal == null)
                    {
                        _logger.LogWarning("Meal {MealId} not found in repository", orderItem.MealId);
                        continue;
                    }

                    var mealSelection = new DailyMealSelection
                    {
                        ClientId = clientOrder.ClientId,
                        ClientSubscriptionId = clientOrder.ClientSubscriptionId,
                        Date = dailySelection.Date,
                        DailySelectionId = dailySelection.Id,
                        MealId = meal.Id,
                        TotalCalories = meal.Calories,
                        TotalProteins = meal.Protein,
                        TotalFats = meal.Fat,
                        BasePrice = (decimal)meal.PriceForCustomer,
                        TotalCarbohydrates = meal.Carbs,
                        MealSelectionType = MealSelectionType.AddOn,
                        Qty = orderItem.QuantityPerDay
                    };

                    // Update daily selection totals
                    dailySelection.TotalCalories += meal.Calories;
                    dailySelection.TotalProteins += meal.Protein;
                    dailySelection.TotalFats += meal.Fat;
                    dailySelection.TotalCarbohydrates += meal.Carbs;

                    await _dbContext.DailyMealSelections.AddAsync(mealSelection);
                    createdSelections++;
                }
            }

            // 5. Save all changes
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully created {CreatedCount} meal selections for order {OrderId}",
                createdSelections, orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create daily meal selections for order {OrderId}", orderId);
            throw; // Re-throw for global error handling
        }
    }

    public async Task SyncSubscriptionStatusesAsync()
    {
        using var scope = _logger.BeginScope("Syncing subscription statuses");
        var now = DateOnly.FromDateTime(DateTime.UtcNow);

        await ExpireSubscriptionsAsync(now);
        await ProcessAutoRenewalsAsync(now);
    }

    /// <summary>
    /// Finds and marks subscriptions as Expired if their EndDate has passed.
    /// </summary>
    private async Task ExpireSubscriptionsAsync(DateOnly now)
    {
        // Fetches only the subscriptions that are active but whose end date is in the past.
        var subscriptionsToExpire = await _dbContext.ClientSubscriptions
            .Where(s => s.SubscriptionStatus == SubscriptionStatus.Active && s.EndDate < now)
            .Include(s => s.Client)
            .ToListAsync();

        if (subscriptionsToExpire.Count == 0) return;

        _logger.LogInformation("Found {Count} subscriptions to expire.", subscriptionsToExpire.Count);

        foreach (var subscription in subscriptionsToExpire)
        {
            subscription.SubscriptionStatus = SubscriptionStatus.Expired;
            subscription.CancelAt = DateTime.UtcNow; // Use a more descriptive property if available

            // Only update the client's main status if this is their current subscription
            if (subscription.Client != null && subscription.Client.ClientSubscriptionId == subscription.Id)
            {
                subscription.Client.SubscriptionStatus = SubscriptionStatus.Expired;
                subscription.Client.ClientSubscriptionId = null;
            }
        }
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Finds subscriptions due for auto-renewal, processes the payment, and creates a new subscription record.
    /// </summary>
    private async Task ProcessAutoRenewalsAsync(DateOnly now)
    {
        // Fetches only subscriptions that are active, enabled for auto-renewal, and due today.
        var subscriptionsToRenew = await _dbContext.ClientSubscriptions
            .Where(s => s.SubscriptionStatus == SubscriptionStatus.Active &&
                        s.IsAutoRenewalEnabled &&
                        s.NextRenewalDate == now)
            .Include(s => s.Client)
            .Include(s => s.SelectedMealTypes)
            .ToListAsync();

        if (subscriptionsToRenew.Count == 0) return;

        _logger.LogInformation("Found {Count} subscriptions to auto-renew.", subscriptionsToRenew.Count);

        foreach (var oldSubscription in subscriptionsToRenew)
        {
            // Use a transaction to ensure the entire renewal process is atomic
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // ✅ ADD THIS CHECK HERE
                // Validate that this is still the client's current subscription.
                // If not, it means a renewal has already happened (e.g., manually)
                // and we should skip this record to avoid a duplicate charge.
                if (oldSubscription.Client?.ClientSubscriptionId != oldSubscription.Id)
                {
                    _logger.LogWarning("Skipping renewal for subscription {SubscriptionId}: It is no longer the client's primary subscription. A renewal may have already occurred.", oldSubscription.Id);
                    await transaction.RollbackAsync();
                    continue; // Skip to the next subscription
                }

                var defaultPaymentMethod = await _dbContext.ClientPaymentMethods
                    .FirstOrDefaultAsync(x => x.IsDefault && x.ClientId == oldSubscription.ClientId);

                if (defaultPaymentMethod == null || oldSubscription.Client?.StripeId == null)
                {
                    _logger.LogWarning("Skipping renewal for subscription {SubscriptionId}: No default payment method or Stripe customer ID found.", oldSubscription.Id);
                    await transaction.RollbackAsync(); // Not strictly necessary here, but good practice
                    continue;
                }

                // 1. Create the new subscription entity
                var newSubscription = CreateNewSubscriptionFromOld(oldSubscription);

                // 2. Calculate new dates and cost
                CalculateRenewalDatesAndCost(newSubscription, oldSubscription);

                // 3. Process OFF-SESSION payment with Stripe
                var description = $"Auto-renewal for {newSubscription.SubscriptionType} Subscription";
                // IMPORTANT: This Stripe service method must be designed for off-session payments
                var paymentIntent = await _stripeService.CreateAddOnPaymentLink(
                    newSubscription.Id.ToString(),
                    defaultPaymentMethod.StripeId,
                    oldSubscription.Client.StripeId,
                    newSubscription.TotalCost,
                    description);

                if (paymentIntent?.Status != "succeeded")
                {
                    _logger.LogError("Payment failed for renewal of subscription {SubscriptionId}. Status: {Status}", oldSubscription.Id, paymentIntent?.Status);
                    await transaction.RollbackAsync(); // Rollback on payment failure
                    continue; // Move to the next subscription
                }

                _logger.LogInformation("Payment successful for new subscription {SubscriptionId}", newSubscription.Id);

                // 4. Update entities and create related records
                newSubscription.PaymentStatus = PaymentStatus.Paid;
                newSubscription.SubscriptionStatus = SubscriptionStatus.Active;
                newSubscription.PaymentDate = SystemTime.Now();
                newSubscription.StripeSubscriptionId = paymentIntent.Id;

                oldSubscription.Client.ClientSubscriptionId = newSubscription.Id;
                oldSubscription.Client.SubscriptionStatus = SubscriptionStatus.Active;

                // Mark the old one as expired so it's not picked up again
                oldSubscription.SubscriptionStatus = SubscriptionStatus.Expired;

                await _dbContext.ClientSubscriptions.AddAsync(newSubscription);

                var payment = CreatePaymentRecord(oldSubscription.Client, newSubscription, paymentIntent, description);
                await _dbContext.Payments.AddAsync(payment);

                await AddLoyaltyPoints(oldSubscription.Client, payment, description);

                // 5. Commit the transaction
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                // 6. Enqueue background jobs AFTER the transaction is committed
                _jobService.Enqueue<ISubscriptionMealOrchestrator>(x => x.CreateDailySelectionsForSubscriptionAsync(newSubscription.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process renewal for subscription {SubscriptionId}. Rolling back transaction.", oldSubscription.Id);
                await transaction.RollbackAsync();
            }
        }
    }

    /// <summary>
    /// Creates a new ClientSubscription instance based on an old one for renewal.
    /// </summary>
    /// <param name="oldSubscription">The subscription to be renewed.</param>
    /// <returns>A new ClientSubscription instance ready for processing.</returns>
    private ClientSubscription CreateNewSubscriptionFromOld(ClientSubscription oldSubscription)
    {
        return new ClientSubscription
        {
            ClientId = oldSubscription.ClientId,
            MealPlanId = oldSubscription.MealPlanId,
            ClientLocationId = oldSubscription.ClientLocationId,
            SubscriptionType = oldSubscription.SubscriptionType,
            SelectedDeliveryDays = oldSubscription.SelectedDeliveryDays,
            PreferredDeliveryTime = oldSubscription.PreferredDeliveryTime,
            IsAutoRenewalEnabled = oldSubscription.IsAutoRenewalEnabled,
            RenewalCount = oldSubscription.RenewalCount + 1,
            PaymentStatus = PaymentStatus.Pending,
            SubscriptionStatus = SubscriptionStatus.Pending,
            AverageCalories = oldSubscription.AverageCalories,
            // Deep copy meal selections to prevent Entity Framework tracking issues
            SelectedMealTypes = oldSubscription.SelectedMealTypes.Select(s => new MealTypeSelection
            {
                QuantityPerDay = s.QuantityPerDay,
                MealTypeId = s.MealTypeId,
                Price = s.Price,
                MealTypeNameAr = s.MealTypeNameAr,
                MealTypeNameEn = s.MealTypeNameEn,
            }).ToList()
        };
    }

    /// <summary>
    /// Calculates the start date, end date, and total cost for a renewed subscription.
    /// </summary>
    /// <param name="newSubscription">The new subscription entity to calculate values for.</param>
    private void CalculateRenewalDatesAndCost(ClientSubscription newSubscription, ClientSubscription oldSubscription)
    {
        // The new subscription starts the day after the old one ends.
        newSubscription.StartDate = oldSubscription.EndDate.AddDays(1);

        // Calculate the base weekly cost from the selected meals.
        decimal weeklyCost = newSubscription.SelectedMealTypes.Sum(s => s.QuantityPerDay * s.Price);
        weeklyCost *= newSubscription.SelectedDeliveryDays.Count;

        // Determine the EndDate and TotalCost based on the subscription type.
        switch (newSubscription.SubscriptionType)
        {
            case SubscriptionType.OneWeek:
                newSubscription.EndDate = newSubscription.StartDate.AddDays(6);
                newSubscription.TotalCost = weeklyCost;
                break;

            case SubscriptionType.OneMonth:
                newSubscription.EndDate = newSubscription.StartDate.AddDays(27);
                newSubscription.TotalCost = weeklyCost * 4; // 4 weeks in a subscription month

                var monthlyDiscount = _clientPortalSettingservice.GetMonthlyPackageSubsciptionDiscount().Result;
                if (monthlyDiscount > 0)
                {
                    newSubscription.TotalCost -= newSubscription.TotalCost * monthlyDiscount / 100;
                }
                break;

            case SubscriptionType.ThreeMonths:
                newSubscription.EndDate = newSubscription.StartDate.AddDays((28 * 3) - 1);
                newSubscription.TotalCost = weeklyCost * 12; // 12 weeks in a 3-month package

                var threeMonthlyDiscount = _clientPortalSettingservice.GetThreeMonthlyPackageSubsciptionDiscount().Result;
                if (threeMonthlyDiscount > 0)
                {
                    newSubscription.TotalCost -= newSubscription.TotalCost * threeMonthlyDiscount / 100;
                }
                break;

            default:
                // Handle unknown subscription type as a single week to prevent errors.
                newSubscription.EndDate = newSubscription.StartDate.AddDays(6);
                newSubscription.TotalCost = weeklyCost;
                _logger.LogWarning("Unhandled SubscriptionType '{SubscriptionType}' in cost calculation. Defaulting to one week.", newSubscription.SubscriptionType);
                break;
        }

        // Set the next renewal date for the newly created subscription
        // Note: You may need to inject your IClientPortalSettingservice to get the offset days.
        var offsetSubscriptionInDays = _clientPortalSettingservice.GetOffsetSubscriptionInDays().Result;
        newSubscription.NextRenewalDate = newSubscription.EndDate.AddDays(1 - offsetSubscriptionInDays);
    }
    

    private Payment CreatePaymentRecord(Client client, ClientSubscription subscription, PaymentIntentResponseDTO paymentIntent, string reason)
    {
        _logger.LogDebug("Creating payment record for payment intent {PaymentIntentId}", paymentIntent.Id);
        return new Payment
        {
            ClientId = client.Id,
            ClientSubscriptionId = subscription.Id,
            InvoiceNumber = paymentIntent.Id,
            TransactionId = paymentIntent.LatestChargeId ?? paymentIntent.Id,
            PaymentStatus = PaymentStatus.Paid,
            PaymentMethod = paymentIntent.PaymentMethodType ?? "Card",
            Amount = paymentIntent.Amount / 100m,
            PaymentGateway = "Stripe",
            Reason = reason,
            Currency = paymentIntent.Currency,
            PaymentDate = SystemTime.Now(),
            OrderId = paymentIntent.OrderId
        };
    }

    private async Task AddLoyaltyPoints(Client client, Payment payment, string reason)
    {
        _logger.LogDebug("Calculating loyalty points for payment {PaymentId}", payment.Id);

        var pointsPerMoney = await _clientPortalSettingservice.GetOnePointEqualInMoney();
        var pointsEarned = pointsPerMoney == 0 ? 0 : (int)(payment.Amount / pointsPerMoney);

        if (pointsEarned > 0)
        {
            await _dbContext.ClientLoyaltyPoints.AddAsync(new ClientLoyaltyPoint
            {
                ClientId = client.Id,
                Reason = reason,
                Source = "Payment",
                TransactionId = payment.Id.ToString(),
                Points = pointsEarned,
                Type = TransactionType.Earn,
                DateEarned = SystemTime.Now(),
            });

            _logger.LogInformation("Awarded {PointsEarned} loyalty points to client {ClientId}", pointsEarned, client.Id);
        }
    }


}
