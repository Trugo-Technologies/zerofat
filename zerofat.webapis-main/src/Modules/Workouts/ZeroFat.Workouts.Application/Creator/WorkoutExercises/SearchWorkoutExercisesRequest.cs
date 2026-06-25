using MediatR;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.WorkoutExercises;
public class SearchWorkoutExercisesRequest : PaginationFilter, IQuery<PaginationResponse<WorkoutExerciseSimplifyDto>>
{
    public DefaultIdType? WorkoutId { get; set; }
    public DefaultIdType? ExerciseId { get; set; }
}


public class SearchWorkoutExercisesRequestHandler(IReadRepository<WorkoutExercise> repository) : IRequestHandler<SearchWorkoutExercisesRequest, PaginationResponse<WorkoutExerciseSimplifyDto>>
{
    private readonly IReadRepository<WorkoutExercise> _repository = repository;

    public async Task<PaginationResponse<WorkoutExerciseSimplifyDto>> Handle(SearchWorkoutExercisesRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new WorkoutExercisesBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
