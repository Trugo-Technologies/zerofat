using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.PlanSchedules;

public class DeletePlanScheduleRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeletePlanScheduleRequest(DefaultIdType id)
    {
        Id = id;
    }
}


public class DeletePlanScheduleRequestHandler(IRepositoryWithEvents<PlanSchedule> repository, IStringLocalizer<DeletePlanScheduleRequestHandler> localizer) : IRequestHandler<DeletePlanScheduleRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<PlanSchedule> _repository = repository;
    private readonly IStringLocalizer<DeletePlanScheduleRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeletePlanScheduleRequest request, CancellationToken cancellationToken)
    {
        var schedule = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = schedule ?? throw new NotFoundException(_localizer["Plan schedule not found"]);

        await _repository.DeleteAsync(schedule, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(schedule.Id);
    }

}
