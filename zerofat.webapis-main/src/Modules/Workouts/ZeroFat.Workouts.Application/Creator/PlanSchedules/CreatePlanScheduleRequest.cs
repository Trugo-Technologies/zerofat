using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Application.Creator.PlanSchedules;
public class CreatePlanScheduleRequest : ICommand<Result<DefaultIdType>>
{
    public int Day { get; set; }
    public Daytime? Daytime { get; set; }
    public DefaultIdType PlanId { get; set; }
    public DefaultIdType? WorkoutId { get; set; }
}

public class CreatePlanScheduleRequestValidator : CustomValidator<CreatePlanScheduleRequest>
{
    public CreatePlanScheduleRequestValidator(IReadRepository<PlanSchedule> repository, IReadRepository<Workout> workoutRepo, IStringLocalizer<CreatePlanScheduleRequestValidator> localaizer)
    {

        RuleFor(u => u.WorkoutId)
               .Cascade(CascadeMode.Stop)
               .NotEmpty()
               .MustAsync(async (id, _) => await workoutRepo.AnyAsync(new ExpressionSpecification<Workout>(x => x.Id == id), _))
                    .WithMessage(localaizer["Workout not found"]);

        RuleFor(u => u.Day)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .GreaterThan(0);
    }
}


public class CreatePlanScheduleRequestHandler(IRepositoryWithEvents<PlanSchedule> repository, IReadRepository<Plan> planRepo, IStringLocalizer<CreatePlanScheduleRequestHandler> localizer) : IRequestHandler<CreatePlanScheduleRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<PlanSchedule> _repository = repository;
    private readonly IStringLocalizer<CreatePlanScheduleRequestHandler> _localizer = localizer;
    private readonly IReadRepository<Plan> _planRepo = planRepo;

    public async Task<Result<DefaultIdType>> Handle(CreatePlanScheduleRequest request, CancellationToken cancellationToken)
    {
        var plan = await _planRepo.GetByIdAsync(request.PlanId, cancellationToken);
        _ = plan ?? throw new NotFoundException(_localizer["Plan not found"]);
        if (request.Day > plan.DaysPerWeek)
            throw new BadRequestException(_localizer["The plan is only {0} days", plan.DaysPerWeek]);

        if (plan.RestDays.Exists(day => day == request.Day))
        {
            throw new BadRequestException(_localizer["You can not add workout to the rest day"]);
        }

        var schedule = new PlanSchedule
        {
            Day = request.Day,
            Index = 1,
            Daytime = request.Daytime,
            PlanId = request.PlanId,
            WorkoutId = request.WorkoutId,
        };

        var schedules = await _repository.ListAsync(new ExpressionSpecification<PlanSchedule>(x => x.PlanId == request.PlanId && x.Day == request.Day), cancellationToken);

        if (schedules.Count != 0)
            schedule.Index = schedules.Max(schedule => schedule.Index) + 1;


        await _repository.AddAsync(schedule, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(schedule.Id);
    }

}
