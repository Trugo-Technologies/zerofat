using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.WorkoutExercises;
public class WorkoutExercisesBySearchRequestSpec : EntitiesByPaginationFilterSpec<WorkoutExercise, WorkoutExerciseSimplifyDto>
{
    public WorkoutExercisesBySearchRequestSpec(SearchWorkoutExercisesRequest request)
        : base(request)
    {
        Query.OrderBy(c => c.Index, !request.HasOrderBy()).ThenBy(x => x.SetIndex, !request.HasOrderBy())
             .Where(x => x.ExerciseId == request.ExerciseId, request.ExerciseId.HasValue)
             .Where(x => x.WorkoutId == request.WorkoutId, request.WorkoutId.HasValue);
    }
}

public class WorkoutExerciseByIdSpec<T> : Specification<WorkoutExercise, T>
{
    public WorkoutExerciseByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

