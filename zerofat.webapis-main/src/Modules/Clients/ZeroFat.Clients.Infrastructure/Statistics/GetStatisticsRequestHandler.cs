using MediatR;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Application.Statistics;
using ZeroFat.ClientPortal.Infrastructure.Persistence.Context;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Infrastructure.Statistics;

public class GetStatisticsRequestHandler(ClientPortalContext db) : IRequestHandler<GetStatisticsRequest, Result<StatisticsDto>>
{
    private readonly ClientPortalContext _db = db;

    public async Task<Result<StatisticsDto>> Handle(GetStatisticsRequest request, CancellationToken cancellationToken)
    {

        return await Result<StatisticsDto>.SuccessAsync(new StatisticsDto
        {
            Clients = await _db.Clients.CountAsync(x => x.DeletedOn == null && x.IsActive, cancellationToken),
            FemaleCount = await _db.Clients.CountAsync(x => x.Gender == Gender.Female && x.DeletedOn == null && x.IsActive, cancellationToken),
            MaleCount = await _db.Clients.CountAsync(x => x.Gender == Gender.Male && x.DeletedOn == null && x.IsActive, cancellationToken),


            Subscriptions = await _db.ClientSubscriptions.CountAsync(x => x.DeletedOn == null && x.SubscriptionStatus == SubscriptionStatus.Active, cancellationToken),
            RecurringSubscriptions = await _db.ClientSubscriptions.CountAsync(x => x.IsAutoRenewalEnabled && x.DeletedOn == null && x.SubscriptionStatus == SubscriptionStatus.Active, cancellationToken),
            SubscriptionsByType = await GetSubscriptionsByType(),

            // Chats = await _db.ClientChats.CountAsync(cancellationToken),
            // Payments = await _db.Payments.CountAsync(x => x.DeletedOn == null, cancellationToken),
            // Locations = await _db.ClientLocations.CountAsync(x => x.DeletedOn == null, cancellationToken),
            // PaymentsByStatus = await GetPaymentsByStatus()
            // SubscriptionByStatus = await GetSubscriptionByStatus(),

            ClientsByDietitianGoal = await GetClientsByDietitianGoal(),

            TotalPaymentsLastMonth = (int)await _db.Payments.Where(x => x.PaymentDate >= DateTime.UtcNow.AddMonths(-1) && x.DeletedOn == null).SumAsync(x => x.Amount, cancellationToken),
        });
    }


    private async Task<Dictionary<string, int>> GetClientsByDietitianGoal()
    {
        return await _db.Clients.Where(x => x.DeletedOn == null && x.IsActive)
                               .GroupBy(x => x.DietitianGoal)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }

    private async Task<Dictionary<string, int>> GetSubscriptionsByType()
    {
        return await _db.ClientSubscriptions.Where(x => x.DeletedOn == null && x.SubscriptionStatus == SubscriptionStatus.Active)
                               .GroupBy(x => x.SubscriptionType)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    } 
    
    private async Task<Dictionary<string, int>> GetSubscriptionByStatus()
    {
        return await _db.ClientSubscriptions.Where(x => x.DeletedOn == null)
                               .GroupBy(x => x.SubscriptionStatus)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }

    private async Task<Dictionary<string, int>> GetPaymentsByStatus()
    {
        return await _db.Payments.Where(x => x.DeletedOn == null)
                               .GroupBy(x => x.PaymentStatus)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }
}
