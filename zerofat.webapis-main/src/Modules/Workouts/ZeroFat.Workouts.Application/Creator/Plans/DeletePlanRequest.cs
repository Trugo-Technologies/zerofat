using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.Plans;

public class DeletePlanRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeletePlanRequest(DefaultIdType id) => Id = id;
}


public class DeletePlanRequestHandler(IRepository<Plan> repository, IStringLocalizer<DeletePlanRequestHandler> localizer, IRepository<PlanSchedule> planScRepo) : IRequestHandler<DeletePlanRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Plan> _repository = repository;
    private readonly IRepository<PlanSchedule> _planScRepo = planScRepo;
    private readonly IStringLocalizer<DeletePlanRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeletePlanRequest request, CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = plan ?? throw new NotFoundException(_localizer["Plan not found"]);

        var schedules = await _planScRepo.ListAsync(new ExpressionSpecification<PlanSchedule>(x => x.PlanId == plan.Id), cancellationToken);
        //if (used)
        //    throw new BadRequestException(_localizer["Can not be deleted, because the plan has schedules"]);

        await _planScRepo.DeleteRangeAsync(schedules, cancellationToken);

        await _repository.DeleteAsync(plan, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(plan.Id);
    }

}
