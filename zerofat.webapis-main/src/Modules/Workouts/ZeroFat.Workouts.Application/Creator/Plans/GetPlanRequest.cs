using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.Plans;
public class GetPlanRequest : IQuery<Result<PlanDetailsDto>>
{
    public DefaultIdType Id { get; set; }
    public bool? WithSchedule { get; set; }
    public GetPlanRequest(Guid id)
    {
        Id = id;
    }
}

public class GetPlanRequestHandler(IRepositoryWithEvents<Plan> repository, IStringLocalizer<GetPlanRequestHandler> localizer, IReadRepository<PlanSchedule> scheduleRepo) : IRequestHandler<GetPlanRequest, Result<PlanDetailsDto>>
{
    private readonly IRepositoryWithEvents<Plan> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<PlanDetailsDto>> Handle(GetPlanRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new PlanByIdSpec<PlanDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Plan not found", request.Id]);

        return await Result<PlanDetailsDto>.SuccessAsync(entity);
    }

}
