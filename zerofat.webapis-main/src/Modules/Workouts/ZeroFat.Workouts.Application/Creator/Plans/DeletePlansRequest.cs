using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.Plans;
public class DeletePlansRequest : ICommand<Result<bool>>
{
    public List<DefaultIdType> Ids { get; set; } = [];
}

public class DeletePlansRequestHandler(
    IRepositoryWithEvents<Plan> repository,
    IStringLocalizer<DeletePlansRequestHandler> localizer,
    IRepositoryWithEvents<PlanSchedule> planScheduleRepo) : IRequestHandler<DeletePlansRequest, Result<bool>>
{

    public async Task<Result<bool>> Handle(DeletePlansRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var plan = await repository.GetByIdAsync(ingId, cancellationToken);
            if (plan != null)
            {
                var schedules = await planScheduleRepo.ListAsync(new ExpressionSpecification<PlanSchedule>(x => x.PlanId == plan.Id), cancellationToken);

                await planScheduleRepo.DeleteRangeAsync(schedules, withSaveChanges: false, cancellationToken: cancellationToken);

                await repository.DeleteAsync(plan, withSaveChanges: false, cancellationToken: cancellationToken);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return await Result<bool>.SuccessAsync(data: true);
    }

}
