using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.Trainers;
public class TrainersBySearchRequestSpec : EntitiesByPaginationFilterSpec<Trainer, TrainerDto>
{
    public TrainersBySearchRequestSpec(SearchTrainersRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.Type == request.Type, request.Type.HasValue)
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class TrainerByIdSpec<T> : Specification<Trainer, T>
{
    public TrainerByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

