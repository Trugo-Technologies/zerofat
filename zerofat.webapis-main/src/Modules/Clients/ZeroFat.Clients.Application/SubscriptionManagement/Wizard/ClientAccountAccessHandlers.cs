using Mapster;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.ClientManagement;
using ZeroFat.ClientPortal.Application.ClientManagement.Clients;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

public class GetClientAccountAccessRequest(DefaultIdType clientId) : IQuery<Result<ClientAccountAccessDto>>
{
    public DefaultIdType ClientId { get; set; } = clientId;
}

public class GetClientAccountAccessRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<DailyMealSelection> dailyMealSelectionRepo,
    IStringLocalizer<GetClientAccountAccessRequestHandler> localizer) : IQueryHandler<GetClientAccountAccessRequest, Result<ClientAccountAccessDto>>
{
    public async Task<Result<ClientAccountAccessDto>> Handle(GetClientAccountAccessRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);

        var client = await clientRepo.FirstOrDefaultAsync(new ClientByIdSpec<ClientDetailsDto>(request.ClientId), cancellationToken)
            ?? throw new NotFoundException(localizer["Client not found"]);

        var dto = new ClientAccountAccessDto
        {
            ClientId = client.Id,
            FullName = client.FullName,
            Email = client.Email,
            Mobile = client.Mobile,
            Gender = client.Gender,
            BirthDate = client.BirthDate,
            IsActive = client.IsActive,
            BMI = client.BMI,
            BMR = client.BMR,
            BodyFat = client.BodyFat,
            TDEE = client.TDEE,
            HeightInCM = client.HeightInCM,
            WeightInKG = client.WeightInKG,
            TargetWeightInKG = client.TargetWeightInKG,
            DietitianGoal = client.DietitianGoal,
            TimeToReachGoalInDays = client.TimeToReachGoalInDays,
            NeededCaloriesToReachGoal = client.NeededCaloriesToReachGoal,
            Allergens = client.ClientAllergicIds.Select(_ => _.ToString()).ToList()
        };

        if (client.ClientSubscriptionId.HasValue)
        {
            dto.Subscription = await BuildSummaryAsync(
                client.ClientSubscriptionId.Value,
                client.SubscriptionStatus,
                subscriptionRepo,
                mealPlanRepo,
                dailyMealSelectionRepo,
                cancellationToken);
        }

        return await Result<ClientAccountAccessDto>.SuccessAsync(dto);
    }

    internal static async Task<ClientSubscriptionSummaryDto> BuildSummaryAsync(
        DefaultIdType subscriptionId,
        SubscriptionStatus clientStatus,
        IReadRepository<ClientSubscription> subscriptionRepo,
        IReadRepository<MealPlan> mealPlanRepo,
        IReadRepository<DailyMealSelection> dailyMealSelectionRepo,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionRepo.FirstOrDefaultAsync(
            new ClientSubscriptionByIdSpec(subscriptionId),
            cancellationToken);

        if (subscription == null)
        {
            return new ClientSubscriptionSummaryDto { SubscriptionStatus = clientStatus };
        }

        var mealPlan = await mealPlanRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<MealPlan>(x => x.Id == subscription.MealPlanId),
            cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var remainingDays = subscription.EndDate >= today
            ? subscription.EndDate.DayNumber - today.DayNumber
            : 0;

        var deliveredMeals = await dailyMealSelectionRepo.CountAsync(
            new ExpressionSpecification<DailyMealSelection>(x =>
                x.ClientSubscriptionId == subscriptionId && x.Date <= today),
            cancellationToken);

        return new ClientSubscriptionSummaryDto
        {
            SubscriptionId = subscription.Id,
            SubscriptionStatus = subscription.SubscriptionStatus,
            ActiveMealPlanName = mealPlan?.NameEn,
            AverageCalories = subscription.AverageCalories,
            MealsPerDay = subscription.SelectedMealTypes.Sum(x => x.QuantityPerDay),
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            RemainingDays = Math.Max(remainingDays, 0),
            TotalDeliveredMeals = deliveredMeals
        };
    }
}

public class GetClientSubscriptionSummaryRequest(DefaultIdType clientId) : IQuery<Result<ClientSubscriptionSummaryDto>>
{
    public DefaultIdType ClientId { get; set; } = clientId;
}

public class GetClientSubscriptionSummaryRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<DailyMealSelection> dailyMealSelectionRepo,
    IStringLocalizer<GetClientSubscriptionSummaryRequestHandler> localizer) : IQueryHandler<GetClientSubscriptionSummaryRequest, Result<ClientSubscriptionSummaryDto>>
{
    public async Task<Result<ClientSubscriptionSummaryDto>> Handle(GetClientSubscriptionSummaryRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var client = await clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == request.ClientId), cancellationToken)
            ?? throw new NotFoundException(localizer["Client not found"]);

        if (!client.ClientSubscriptionId.HasValue)
        {
            return await Result<ClientSubscriptionSummaryDto>.SuccessAsync(new ClientSubscriptionSummaryDto
            {
                SubscriptionStatus = client.SubscriptionStatus
            });
        }

        var summary = await GetClientAccountAccessRequestHandler.BuildSummaryAsync(
            client.ClientSubscriptionId.Value,
            client.SubscriptionStatus,
            subscriptionRepo,
            mealPlanRepo,
            dailyMealSelectionRepo,
            cancellationToken);

        return await Result<ClientSubscriptionSummaryDto>.SuccessAsync(summary);
    }
}
