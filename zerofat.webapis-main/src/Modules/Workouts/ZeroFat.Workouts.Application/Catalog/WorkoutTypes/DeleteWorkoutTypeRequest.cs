using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.WorkoutTypes;
public class DeleteWorkoutTypeRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteWorkoutTypeRequest(DefaultIdType id) => Id = id;
}


public class DeleteWorkoutTypeRequestHandler(IRepository<WorkoutType> repository, IStringLocalizer<DeleteWorkoutTypeRequestHandler> localizer, IReadRepository<Workout> workoutRepo) : IRequestHandler<DeleteWorkoutTypeRequest, Result<DefaultIdType>>
{
    private readonly IRepository<WorkoutType> _repository = repository;
    private readonly IReadRepository<Workout> _workoutRepo = workoutRepo;
    private readonly IStringLocalizer<DeleteWorkoutTypeRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteWorkoutTypeRequest request, CancellationToken cancellationToken)
    {
        var type = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = type ?? throw new NotFoundException(_localizer["Type not found"]);

        bool used = await _workoutRepo.AnyAsync(new ExpressionSpecification<Workout>(x => x.WorkoutTypeId == type.Id), cancellationToken);
        if (used)
            throw new BadRequestException(_localizer["Can not be deleted, because it's linked with workouts"]);

        await _repository.DeleteAsync(type, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(type.Id);
    }

}
