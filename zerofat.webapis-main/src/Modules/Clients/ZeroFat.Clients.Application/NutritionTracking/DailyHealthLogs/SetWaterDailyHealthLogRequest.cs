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
public class SetWaterDailyHealthLogRequest : ICommand<Result<DefaultIdType>>
{
    public double? Value { get; set; }
    public DateOnly Date { get; set; }
}


public class SetWaterDailyHealthLogRequestValidator : CustomValidator<SetWaterDailyHealthLogRequest>
{
    public SetWaterDailyHealthLogRequestValidator()
    {

        RuleFor(x => x.Value).NotNull().NotEmpty();
        RuleFor(x => x.Date).NotNull().NotEmpty();
    }
}


public class SetWaterDailyHealthLogRequestHandler(
    IRepository<DailyHealthLog> repo,
    ICurrentUser currentUser,
    IStringLocalizer<SetWaterDailyHealthLogRequestHandler> localizer) : IRequestHandler<SetWaterDailyHealthLogRequest, Result<DefaultIdType>>
{


    public async Task<Result<DefaultIdType>> Handle(SetWaterDailyHealthLogRequest request, CancellationToken cancellationToken)
    {
        bool isClient = currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (!isClient)
        {
            throw new BadRequestException(localizer["only client can set health log."]);
        }

        var stat = await repo.FirstOrDefaultAsync(new ExpressionSpecification<DailyHealthLog>(x => x.LogDate == request.Date && x.ClientId == currentUser.GetUserId()), cancellationToken);
        if (stat != null)
        {
            stat.RecordWaterIntake(new WaterIntake(request.Value.GetValueOrDefault()));
            await repo.UpdateAsync(stat, cancellationToken);
        }
        else
        {
            stat = new DailyHealthLog
            {
                ClientId = currentUser.GetUserId(),
                LogDate = request.Date,
            };

            stat.RecordWaterIntake(new WaterIntake(request.Value.GetValueOrDefault()));
            await repo.AddAsync(stat, cancellationToken);
        }

        return await Result<DefaultIdType>.SuccessAsync(stat.Id);
    }

}
