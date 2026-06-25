using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.GymUp.Domain.Creator;

namespace ZeroFat.GymUp.Application.Creator.ClientWorkouts;
public class ClientWorkoutsBySearchRequestSpec : EntitiesByPaginationFilterSpec<ClientWorkout, ClientWorkoutDetailsDto>
{
    public ClientWorkoutsBySearchRequestSpec(SearchClientWorkoutsRequest request)
        : base(request)
    {
        Query.Where(x => x.WorkoutId == request.WorkoutId, request.WorkoutId.HasValue)
             .Where(x => x.UserId == request.UserId, request.UserId.HasValue)
             .OrderByDescending(x => x.CreatedOn, !request.HasOrderBy());
    }
}

public class ClientWorkoutByIdSpec<T> : Specification<ClientWorkout, T>
{
    public ClientWorkoutByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

public class ClientWorkoutBySpec<T> : Specification<ClientWorkout, T>
{
    public ClientWorkoutBySpec(DefaultIdType? userId, DefaultIdType? workoutId, DateOnly? date)
    {
        Query.Where(p => p.UserId == userId, userId.HasValue)
             .Where(p => p.WorkoutId == workoutId, workoutId.HasValue)
             .Where(p => p.Date == date, date.HasValue);
    }
}

