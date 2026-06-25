using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.BodyParts;

public class DeleteBodyPartRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteBodyPartRequest(DefaultIdType id) => Id = id;
}


public class DeleteBodyPartRequestHandler(IRepository<BodyPart> repository, IStringLocalizer<DeleteBodyPartRequestHandler> localizer, IReadRepository<WorkoutBodyPart> workoutBodyPartRepo, IReadRepository<ExerciseBodyPart> exerciseBodyPartRepo) : IRequestHandler<DeleteBodyPartRequest, Result<DefaultIdType>>
{
    private readonly IRepository<BodyPart> _repository = repository;
    private readonly IReadRepository<WorkoutBodyPart> _workoutBodyPartRepo = workoutBodyPartRepo;
    private readonly IReadRepository<ExerciseBodyPart> _exerciseBodyPartRepo = exerciseBodyPartRepo;
    private readonly IStringLocalizer<DeleteBodyPartRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteBodyPartRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["Body part not found"]);

        bool used = await _workoutBodyPartRepo.AnyAsync(new ExpressionSpecification<WorkoutBodyPart>(x => x.BodyPartId == part.Id), cancellationToken);
        if (used)
            throw new BadRequestException(_localizer["Can not be deleted, because it's linked with workouts"]);
         
        used = await _exerciseBodyPartRepo.AnyAsync(new ExpressionSpecification<ExerciseBodyPart>(x => x.BodyPartId == part.Id), cancellationToken);
        if (used)
            throw new BadRequestException(_localizer["Can not be deleted, because it's linked with exercises"]);

        await _repository.DeleteAsync(part, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
