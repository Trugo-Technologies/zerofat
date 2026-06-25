using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.Common.ValueObjects;
using ZeroFat.ClientPortal.Domain.NutritionTracking;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.NutritionTracking.DailyHealthLogs;
public class SetWeightDailyHealthLogRequest : ICommand<Result<DefaultIdType>>
{
    public double? Value { get; set; }
    public DateOnly Date { get; set; }
}


public class SetWeightDailyHealthLogRequestValidator : CustomValidator<SetWeightDailyHealthLogRequest>
{
    public SetWeightDailyHealthLogRequestValidator()
    {

        RuleFor(x => x.Value).NotNull().NotEmpty();
        RuleFor(x => x.Date).NotNull().NotEmpty();
    }
}


public class SetWeightDailyHealthLogRequestHandler(
    IRepository<DailyHealthLog> repo,
    ICurrentUser currentUser,
    IRepository<Client> clientRepo,
    IStringLocalizer<SetWeightDailyHealthLogRequestHandler> localizer) : IRequestHandler<SetWeightDailyHealthLogRequest, Result<DefaultIdType>>
{


    public async Task<Result<DefaultIdType>> Handle(SetWeightDailyHealthLogRequest request, CancellationToken cancellationToken)
    {
        bool isClient = currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (!isClient)
        {
            throw new BadRequestException("only client can set health log.");
        }

        var client = await clientRepo.GetByIdAsync(currentUser.GetUserId(), cancellationToken);
        if (client is null)
            throw new NotFoundException(localizer["Client not found"]);

        var stat = await repo.FirstOrDefaultAsync(new ExpressionSpecification<DailyHealthLog>(x => x.LogDate == request.Date && x.ClientId == currentUser.GetUserId()), cancellationToken);
        if (stat != null)
        {
            stat.RecordWeight(new BodyMeasurement(request.Value.GetValueOrDefault(), HealthMeasurementUnit.Kilograms));
            client.CurrentWeightInKG = request.Value;
            await repo.UpdateAsync(stat, cancellationToken);

            // await clientRepo.UpdateAsync(client, cancellationToken);
        }
        else
        {
            stat = new DailyHealthLog
            {
                ClientId = currentUser.GetUserId(),
                LogDate = request.Date,
            };

            stat.RecordWeight(new BodyMeasurement(request.Value.GetValueOrDefault(), HealthMeasurementUnit.Kilograms));
            client.CurrentWeightInKG = request.Value;

            await repo.AddAsync(stat, cancellationToken);
            await clientRepo.UpdateAsync(client, cancellationToken);
        }


        return await Result<DefaultIdType>.SuccessAsync(stat.Id);
    }

}
