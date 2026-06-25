using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.ExerciseBodyParts;
public class ExerciseBodyPartsBySearchRequestSpec : EntitiesByPaginationFilterSpec<ExerciseBodyPart, ExerciseBodyPartSimplifyDto>
{
    public ExerciseBodyPartsBySearchRequestSpec(SearchExerciseBodyPartsRequest request)
        : base(request)
    {
        Query.Where(x => x.BodyPartId == request.BodyPartId, request.BodyPartId.HasValue)
             .Where(x => x.ExerciseId == request.ExerciseId, request.ExerciseId.HasValue);
    }
}

public class ExerciseBodyPartByIdSpec<T> : Specification<ExerciseBodyPart, T>
{
    public ExerciseBodyPartByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

