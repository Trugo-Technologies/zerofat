using MediatR;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Application.Statistics;
using ZeroFat.GymUp.Infrastructure.Persistence.Context;

namespace ZeroFat.GymUp.Infrastructure.Statistics;

public class GetStatisticsRequestHandler(GymUpContext db) : IRequestHandler<GetStatisticsRequest, Result<StatisticsDto>>
{
    private readonly GymUpContext _db = db;

    public async Task<Result<StatisticsDto>> Handle(GetStatisticsRequest request, CancellationToken cancellationToken)
    {

        return await Result<StatisticsDto>.SuccessAsync(new StatisticsDto
        {
            Plans = await _db.Plans.CountAsync(x => x.DeletedOn == null && x.IsActive, cancellationToken),
            BodyParts = await _db.BodyParts.CountAsync(x => x.DeletedOn == null && x.IsActive, cancellationToken),
            EquipmentCategories = await _db.EquipmentCategories.CountAsync(x => x.DeletedOn == null && x.IsActive, cancellationToken),
            Equipments = await _db.Equipments.CountAsync(x => x.DeletedOn == null && x.IsActive, cancellationToken),
            Exercsises = await _db.Exercises.CountAsync(x => x.DeletedOn == null && x.IsActive, cancellationToken),
            Workouts = await _db.Workouts.CountAsync(x => x.DeletedOn == null && x.IsActive, cancellationToken),
            Reviews = await _db.PlanReviews.CountAsync(x => x.DeletedOn == null, cancellationToken),
            Trainers = await _db.Trainers.CountAsync(x => x.DeletedOn == null && x.IsActive, cancellationToken),
            WorkoutTypes = await _db.WorkoutTypes.CountAsync(x => x.DeletedOn == null && x.IsActive, cancellationToken),
            PlansByLevel = await GetPlansByLevel(),
            PlansByEnvironment = await GetPlansByEnvironment(),
            WorkoutsByFormat = await GetWorkoutsByFormat(),
            MostReviewdPlans = await GetMostReviewdPlans()
        });
    }


    private async Task<Dictionary<string, int>> GetPlansByLevel()
    {
        return await _db.Plans.Where(x => x.DeletedOn == null && x.IsActive)
                               .GroupBy(x => x.Level)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }

    private async Task<Dictionary<string, int>> GetPlansByEnvironment()
    {
        return await _db.Plans.Where(x => x.Environment != null && x.DeletedOn == null && x.IsActive)
                               .GroupBy(x => x.Environment!.Value)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }

    private async Task<Dictionary<string, int>> GetWorkoutsByFormat()
    {
        return await _db.Workouts.Where(x => x.DeletedOn == null && x.IsActive)
                               .GroupBy(x => x.Format)
                               .Select(x => new
                               {
                                   key = x.Key.ToString(),
                                   value = x.Count()
                               })
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }

    private async Task<Dictionary<string, int>> GetMostReviewdPlans()
    {
        return await _db.PlanReviews.Where(x => x.DeletedOn == null && x.Plan.IsActive && x.Plan.DeletedOn == null)
                               .GroupBy(x => x.PlanId)
                               .Select(x => new
                               {
                                   key = x.FirstOrDefault()!.Plan!.NameEn! ?? x.FirstOrDefault()!.Plan!.NameAr!,
                                   value = x.Count()
                               })
                               .OrderByDescending(z => z.value)
                               .Take(10)
                               .ToDictionaryAsync(x => x.key, x => x.value);

    }
}
