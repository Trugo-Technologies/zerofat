using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;

namespace ZeroFat.GymUp.Application.Creator.WorkoutBodyParts;
public class CreateWorkoutBodyPartRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType WorkoutId { get; set; }
    public DefaultIdType BodyPartId { get; set; }
}

public class CreateWorkoutBodyPartRequestValidator : CustomValidator<CreateWorkoutBodyPartRequest>
{
    public CreateWorkoutBodyPartRequestValidator(IReadRepository<WorkoutBodyPart> repository, IReadRepository<BodyPart> bodyRepo, IReadRepository<Workout> workoutRepo, IStringLocalizer<CreateWorkoutBodyPartRequestValidator> localaizer)
    {

        RuleFor(u => u.WorkoutId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await workoutRepo.AnyAsync(new ExpressionSpecification<Workout>(x => x.Id == id), _))
                .WithMessage(localaizer["Workout not found"]);

        RuleFor(u => u.BodyPartId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await bodyRepo.AnyAsync(new ExpressionSpecification<BodyPart>(x => x.Id == id), _))
                .WithMessage(localaizer["Body part not found"]);

        RuleFor(u => true)
           .Cascade(CascadeMode.Stop)
           .MustAsync(async (req, _, c) => !await repository.AnyAsync(new ExpressionSpecification<WorkoutBodyPart>(x => x.BodyPartId == req.BodyPartId && x.WorkoutId == req.WorkoutId), c))
                .WithMessage(localaizer["Body part is already attached with this workout"]);
    }
}


public class CreateWorkoutBodyPartRequestHandler(IRepositoryWithEvents<WorkoutBodyPart> repository) : IRequestHandler<CreateWorkoutBodyPartRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<WorkoutBodyPart> _repository = repository;

    public async Task<Result<DefaultIdType>> Handle(CreateWorkoutBodyPartRequest request, CancellationToken cancellationToken)
    {

        var workoutEq = new WorkoutBodyPart
        {
            BodyPartId = request.BodyPartId,
            WorkoutId = request.WorkoutId
        };

        await _repository.AddAsync(workoutEq, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(workoutEq.Id);
    }

}
