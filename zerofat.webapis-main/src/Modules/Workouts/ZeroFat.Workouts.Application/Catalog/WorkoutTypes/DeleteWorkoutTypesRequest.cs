using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.WorkoutTypes;
public class DeleteWorkoutTypesRequest : ICommand<Result<bool>>
{
    public List<DefaultIdType> Ids { get; set; } = [];
}

public class DeleteWorkoutTypesRequestHandler(
    IRepositoryWithEvents<WorkoutType> repository,
    IStringLocalizer<DeleteWorkoutTypesRequestHandler> localizer,
    IReadRepository<Workout> workoutRepo) : IRequestHandler<DeleteWorkoutTypesRequest, Result<bool>>
{

    public async Task<Result<bool>> Handle(DeleteWorkoutTypesRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var workoutType = await repository.GetByIdAsync(ingId, cancellationToken);
            if (workoutType != null)
            {
                if (await workoutRepo.AnyAsync(new ExpressionSpecification<Workout>(x => x.WorkoutTypeId == workoutType.Id), cancellationToken))
                    continue;

                await repository.DeleteAsync(workoutType, withSaveChanges: false, cancellationToken: cancellationToken);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return await Result<bool>.SuccessAsync(data: true);
    }

}
