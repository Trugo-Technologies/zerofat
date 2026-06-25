using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Domain.Common.ValueObjects;
using ZeroFat.ClientPortal.Domain.NutritionTracking;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.NutritionTracking.DailyHealthLogs;
public class AddCalorieRecordDailyHealthLogRequest : ICommand<Result<DefaultIdType>>
{
    public double? Value { get; set; }
    public string? Name { get; set; }
    public DateOnly Date { get; set; }
    public CalorieRecordType RecordType { get; set; }
}


public class AddCalorieRecordDailyHealthLogRequestValidator : CustomValidator<AddCalorieRecordDailyHealthLogRequest>
{
    public AddCalorieRecordDailyHealthLogRequestValidator()
    {

        RuleFor(x => x.Value).NotNull().NotEmpty();
        RuleFor(x => x.Date).NotNull().NotEmpty();
        RuleFor(x => x.Name).NotNull().NotEmpty();
    }
}


public class AddCalorieRecordDailyHealthLogRequestHandler(
    IRepository<DailyHealthLog> repo,
    ICurrentUser currentUser,
    IRepository<CalorieRecord> calorieRecordRepo,
    IStringLocalizer<AddCalorieRecordDailyHealthLogRequestHandler> localizer) : IRequestHandler<AddCalorieRecordDailyHealthLogRequest, Result<DefaultIdType>>
{


    public async Task<Result<DefaultIdType>> Handle(AddCalorieRecordDailyHealthLogRequest request, CancellationToken cancellationToken)
    {
        bool isClient = currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (!isClient)
        {
            throw new BadRequestException("only client can set health log.");
        }

        var stat = await repo.FirstOrDefaultAsync(new ExpressionSpecification<DailyHealthLog>(x => x.LogDate == request.Date && x.ClientId == currentUser.GetUserId()), cancellationToken);
        if (stat != null)
        {
            var caloriesRecord = new CalorieRecord()
            {
                Calories = request.Value.GetValueOrDefault(),
                DailyHealthLogId = stat.Id,
                Name = request.Name!,
                RecordedAt = DateTime.UtcNow,
                RecordType = request.RecordType,
                Nutrition = new NutritionFacts(request.Value.GetValueOrDefault(), 0, 0, 0),
            };

            switch (request.RecordType)
            {
                case CalorieRecordType.Activity:
                    stat.TotalCaloriesBurned += request.Value.GetValueOrDefault();
                    break;
                case CalorieRecordType.Food:
                    stat.TotalCaloriesConsumed += request.Value.GetValueOrDefault();
                    break;
                default:
                    stat.TotalCaloriesConsumed += request.Value.GetValueOrDefault();
                    break;
            }

            await calorieRecordRepo.AddAsync(caloriesRecord, cancellationToken);

            // await repo.UpdateAsync(stat, cancellationToken);
            // await clientRepo.UpdateAsync(client, cancellationToken);
        }
        else
        {
            stat = new DailyHealthLog
            {
                ClientId = currentUser.GetUserId(),
                LogDate = request.Date,
            };

            var caloriesRecord = new CalorieRecord()
            {
                Calories = request.Value.GetValueOrDefault(),
                DailyHealthLogId = stat.Id,
                Name = request.Name!,
                RecordedAt = DateTime.UtcNow,
                RecordType = request.RecordType,
                Nutrition = new NutritionFacts(request.Value.GetValueOrDefault(), 0, 0, 0),
            };
            stat.AddCalorieRecord(caloriesRecord);

            switch (request.RecordType)
            {
                case CalorieRecordType.Activity:
                    stat.TotalCaloriesBurned += request.Value.GetValueOrDefault();
                    break;
                case CalorieRecordType.Food:
                    stat.TotalCaloriesConsumed += request.Value.GetValueOrDefault();
                    break;
                default:
                    stat.TotalCaloriesConsumed += request.Value.GetValueOrDefault();
                    break;
            }

            await repo.AddAsync(stat, cancellationToken);

        }


        return await Result<DefaultIdType>.SuccessAsync(stat.Id);
    }

}
