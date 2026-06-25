using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.Exercises;
public class ExercisesBySearchRequestSpec : EntitiesByPaginationFilterSpec<Exercise, ExerciseDto>
{
    public ExercisesBySearchRequestSpec(SearchExercisesRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.TrainerId == request.TrainerId, request.TrainerId.HasValue)
             .Where(x => x.Type == request.Type, request.Type.HasValue)
             .Where(x => x.ExerciseBodyParts.Any(z => z.BodyPartId == request.BodyPartId), request.BodyPartId.HasValue)
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class ExerciseByIdSpec<T> : Specification<Exercise, T>
{
    public ExerciseByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

