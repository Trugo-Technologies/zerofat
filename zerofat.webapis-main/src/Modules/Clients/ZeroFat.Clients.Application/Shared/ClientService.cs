using Ardalis.Specification;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Shared;
using ZeroFat.ClientPortal.Application.ClientManagement;
using ZeroFat.ClientPortal.Application.ClientManagement.ClientPaymentMethods;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.Common.ValueObjects;
using ZeroFat.ClientPortal.Domain.NutritionTracking;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.Shared;

public class ClientService : IClientService
{
    private readonly IRepository<Client> _clientRepository;
    private readonly IReadRepository<ClientSubscription> _clientSubscriptionRepository;
    private readonly IReadRepository<ClientPaymentMethod> _clientPaymentMethodRepo;
    private readonly IRepository<DailyHealthLog> _dailyHealthLogRepo;
    private readonly IRepository<CalorieRecord> _calorieRecordRepo;

    public ClientService(
        IRepository<Client> clientRepository,
        IRepository<DailyHealthLog> dailyHealthLogRepo,
        IRepository<CalorieRecord> calorieRecordRepo,
        IReadRepository<ClientPaymentMethod> clientPaymentMethodRepo,
        IReadRepository<ClientSubscription> clientSubscriptionRepository)
    {
        _clientRepository = clientRepository;
        _clientSubscriptionRepository = clientSubscriptionRepository;
        _dailyHealthLogRepo = dailyHealthLogRepo;
        _calorieRecordRepo = calorieRecordRepo;
        _clientPaymentMethodRepo = clientPaymentMethodRepo;
    }

    public async Task<List<DefaultIdType>?> GetClientAllergicIdsByClientId(DefaultIdType clientId)
    {
        var client = await _clientRepository.FirstOrDefaultAsync(new ClientAllergicsByIdSpec(clientId));
        return client;
    }

    public async Task<bool> GetClientStatusByClientId(DefaultIdType clientId)
    {
        var client = await _clientRepository.FirstOrDefaultAsync(new ClientStatusByIdSpec(clientId));
        if (client == null)
            return true;

        return client.IsActive && !client.AccountIsDeleted;
    }

    public async Task DeactivateClientAsync(DefaultIdType clientId)
    {
        var client = await _clientRepository.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == clientId));
        if (client != null)
        {
            client.IsActive = false;
            client.AccountIsDeleted = true;
            await _clientRepository.SaveChangesAsync();
        }
    }


    public async Task<ClientSharedDto?> GetClientById(DefaultIdType clientId)
    {
        var client = await _clientRepository.FirstOrDefaultAsync(new ClientByIdSpec<ClientSharedDto>(clientId));
        var now = DateOnly.FromDateTime(DateTime.Now);

        if (client != null && client.NewGoalStart.HasValue && client.NewGoalStart <= now && client.WeightInKG.HasValue && client.TargetWeightInKG.HasValue && client.CurrentWeightInKG.HasValue)
        {
            int elapsedDays = now.DayNumber - client.NewGoalStart.Value.DayNumber;
            double initialWeight = client.WeightInKG.Value;
            double targetWeight = client.TargetWeightInKG.Value;
            double currentWeight = client.CurrentWeightInKG.Value;
            int totalDuration = client.TimeToReachGoalInDays; // Total duration in days

            double originalRate = Math.Abs(targetWeight - initialWeight) / totalDuration;
            double remainingWeight = Math.Abs(targetWeight - currentWeight);
            int remainingDays = (int)Math.Round(remainingWeight / originalRate);

            if (remainingDays < (totalDuration - elapsedDays))
            {
                client.TimeToReachGoalInDays = elapsedDays + remainingDays;
            }
            else
            {
                client.TimeToReachGoalInDays = totalDuration;
                // Adjust other parameters such as calorie intake if needed
            }
        }

        return client;
    }

    public async Task<ClientPaymentMethodShareDto?> GetClientDefaultPaymentMethod(DefaultIdType clientId)
    {
        return await _clientPaymentMethodRepo.FirstOrDefaultAsync(new DefaultClientPaymentMethodByIdSpec<ClientPaymentMethodShareDto>(clientId));
    }



    public async Task UpdateClientStatisticesFromWorkout(DateOnly date, DefaultIdType clientId, double calories, string nameEn)
    {
        var stat = await _dailyHealthLogRepo.FirstOrDefaultAsync(new ExpressionSpecification<DailyHealthLog>(x => x.LogDate == date && x.ClientId == clientId));
        if (stat != null)
        {
            var caloriesRecord = new CalorieRecord()
            {
                Calories = calories,
                DailyHealthLogId = stat.Id,
                Name = nameEn!,
                RecordedAt = DateTime.UtcNow,
                RecordType = CalorieRecordType.Activity,
                Nutrition = new NutritionFacts(0, 0, 0, 0),
            };

            stat.TotalCaloriesBurned += calories;
            await _calorieRecordRepo.AddAsync(caloriesRecord);

            // await repo.UpdateAsync(stat, cancellationToken);
            // await clientRepo.UpdateAsync(client, cancellationToken);
        }
        else
        {
            stat = new DailyHealthLog
            {
                ClientId = clientId,
                LogDate = date,
            };

            var caloriesRecord = new CalorieRecord()
            {
                Calories = calories,
                DailyHealthLogId = stat.Id,
                Name = nameEn!,
                RecordedAt = DateTime.UtcNow,
                RecordType = CalorieRecordType.Activity,
                Nutrition = new NutritionFacts(0, 0, 0, 0),
            };

            stat.AddCalorieRecord(caloriesRecord);

            stat.TotalCaloriesBurned += calories;

            await _dailyHealthLogRepo.AddAsync(stat);
        }

    }
    public async Task UpdateClientOrEmail(DefaultIdType clientId, string? mail, string? phoneNumber)
    {
        var client = await _clientRepository.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == clientId));
        if (client != null)
        {
            if (mail.HasValue())
            {
                client.Email = mail;
            }

            if (phoneNumber.HasValue())
            {
                client.Mobile = phoneNumber;
            }

            await _clientRepository.SaveChangesAsync();
        }

    }

    public async Task<ClientSubscriptionSharedDto?> GetClientSubscriptionById(DefaultIdType clientSubscriptionId)
    {
        return await _clientSubscriptionRepository.FirstOrDefaultAsync(new ClientSubscriptionByIdSpec<ClientSubscriptionSharedDto>(clientSubscriptionId));
    }
}
