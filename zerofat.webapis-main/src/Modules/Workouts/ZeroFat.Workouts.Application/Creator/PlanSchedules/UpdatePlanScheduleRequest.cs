using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Application.Creator.PlanSchedules;
public class UpdatePlanScheduleRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public int Day { get; set; }
    public int Index { get; set; }
    public Daytime? Daytime { get; set; }
}

public class UpdatePlanScheduleRequestValidator : CustomValidator<UpdatePlanScheduleRequest>
{
    public UpdatePlanScheduleRequestValidator(IReadRepository<PlanSchedule> repository, IReadRepository<Workout> workoutRepo, IStringLocalizer<UpdatePlanScheduleRequestValidator> localaizer)
    {
        RuleFor(u => u.Day)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .GreaterThan(0);

        RuleFor(u => u.Index)
           .Cascade(CascadeMode.Stop)
           .NotEmpty();

    }
}

public class UpdatePlanScheduleRequestHandler(IRepositoryWithEvents<PlanSchedule> repository, IStringLocalizer<UpdatePlanScheduleRequestHandler> localizer, IReadRepository<Plan> planRepo) : IRequestHandler<UpdatePlanScheduleRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<PlanSchedule> _repository = repository;
    private readonly IReadRepository<Plan> _planRepo = planRepo;
    private readonly IStringLocalizer<UpdatePlanScheduleRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(UpdatePlanScheduleRequest request, CancellationToken cancellationToken)
    {
        var schedule = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = schedule ?? throw new NotFoundException(_localizer["Plan schedule not found"]);

        var plan = await _planRepo.GetByIdAsync(schedule.PlanId, cancellationToken);
        _ = plan ?? throw new NotFoundException(_localizer["Plan not found"]);

        if (request.Day > plan.DaysPerWeek)
            throw new BadRequestException(_localizer["The plan is only {0} days", plan.DaysPerWeek]);

        if (request.Index != schedule.Index || request.Day != schedule.Day)
        {
            var set = await _repository.ListAsync(new ExpressionSpecification<PlanSchedule>(x => x.PlanId == schedule.PlanId && x.Day == request.Day && x.Index >= request.Index), cancellationToken);
            foreach (var item in set)
            {
                item.Index++;
            }
        }

        schedule.Day = request.Day;
        schedule.Index = request.Index;
        schedule.Daytime = request.Daytime;

        await _repository.SaveChangesAsync(cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(schedule.Id);
    }
}

