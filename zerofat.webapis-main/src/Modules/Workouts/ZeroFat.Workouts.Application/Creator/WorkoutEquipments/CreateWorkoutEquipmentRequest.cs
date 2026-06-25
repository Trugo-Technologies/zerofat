using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;

namespace ZeroFat.GymUp.Application.Creator.WorkoutEquipments;
public class CreateWorkoutEquipmentRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType WorkoutId { get; set; }
    public DefaultIdType EquipmentId { get; set; }
}

public class CreateWorkoutEquipmentRequestValidator : CustomValidator<CreateWorkoutEquipmentRequest>
{
    public CreateWorkoutEquipmentRequestValidator(IReadRepository<WorkoutEquipment> repository, IReadRepository<Equipment> equRepo, IReadRepository<Workout> workoutRepo, IStringLocalizer<CreateWorkoutEquipmentRequestValidator> localaizer)
    {

        RuleFor(u => u.WorkoutId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await workoutRepo.AnyAsync(new ExpressionSpecification<Workout>(x => x.Id == id), _))
                .WithMessage(localaizer["Workout not found"]);

        RuleFor(u => u.EquipmentId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await equRepo.AnyAsync(new ExpressionSpecification<Equipment>(x => x.Id == id), _))
                .WithMessage(localaizer["Equipment not found"]);

        RuleFor(u => true)
           .Cascade(CascadeMode.Stop)
           .MustAsync(async (req, _, c) => !await repository.AnyAsync(new ExpressionSpecification<WorkoutEquipment>(x => x.EquipmentId == req.EquipmentId && x.WorkoutId == req.WorkoutId), c))
                .WithMessage(localaizer["Equipment is already attached with this workout"]);
    }
}


public class CreateWorkoutEquipmentRequestHandler(IRepositoryWithEvents<WorkoutEquipment> repository) : IRequestHandler<CreateWorkoutEquipmentRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<WorkoutEquipment> _repository = repository;

    public async Task<Result<DefaultIdType>> Handle(CreateWorkoutEquipmentRequest request, CancellationToken cancellationToken)
    {

        var workoutEq = new WorkoutEquipment
        {
            EquipmentId = request.EquipmentId,
            WorkoutId = request.WorkoutId,
        };

        await _repository.AddAsync(workoutEq, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(workoutEq.Id);
    }

}
