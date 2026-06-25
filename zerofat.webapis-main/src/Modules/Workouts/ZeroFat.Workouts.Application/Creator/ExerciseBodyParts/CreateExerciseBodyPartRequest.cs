using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;

namespace ZeroFat.GymUp.Application.Creator.ExerciseBodyParts;

public class CreateExerciseBodyPartRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType ExerciseId { get; set; }
    public DefaultIdType BodyPartId { get; set; }
}

public class CreateExerciseBodyPartRequestValidator : CustomValidator<CreateExerciseBodyPartRequest>
{
    public CreateExerciseBodyPartRequestValidator(IReadRepository<ExerciseBodyPart> repository, IReadRepository<BodyPart> bodyRepo, IReadRepository<Exercise> ExerciseRepo, IStringLocalizer<CreateExerciseBodyPartRequestValidator> localaizer)
    {

        RuleFor(u => u.ExerciseId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await ExerciseRepo.AnyAsync(new ExpressionSpecification<Exercise>(x => x.Id == id), _))
                .WithMessage(localaizer["Exercise not found"]);

        RuleFor(u => u.BodyPartId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await bodyRepo.AnyAsync(new ExpressionSpecification<BodyPart>(x => x.Id == id), _))
                .WithMessage(localaizer["Body part not found"]);

        RuleFor(u => true)
           .Cascade(CascadeMode.Stop)
           .MustAsync(async (req, _, c) => !await repository.AnyAsync(new ExpressionSpecification<ExerciseBodyPart>(x => x.BodyPartId == req.BodyPartId && x.ExerciseId == req.ExerciseId), c))
                .WithMessage(localaizer["Body part is already attached with this exercise"]);
    }
}


public class CreateExerciseBodyPartRequestHandler(IRepositoryWithEvents<ExerciseBodyPart> repository) : IRequestHandler<CreateExerciseBodyPartRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<ExerciseBodyPart> _repository = repository;

    public async Task<Result<DefaultIdType>> Handle(CreateExerciseBodyPartRequest request, CancellationToken cancellationToken)
    {

        var exerciseBody = new ExerciseBodyPart
        {
            BodyPartId = request.BodyPartId,
            ExerciseId = request.ExerciseId
        };

        await _repository.AddAsync(exerciseBody, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(exerciseBody.Id);
    }

}
