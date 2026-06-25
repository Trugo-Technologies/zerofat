using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.NutritionTracking;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientGoals;
public class ClientGoalsBySearchRequestSpec : EntitiesByPaginationFilterSpec<ClientGoal, ClientGoalSimplifyDto>
{
    public ClientGoalsBySearchRequestSpec(SearchClientGoalsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.ClientId == request.ClientId, request.ClientId.HasValue);
    }
}

public class ClientGoalByIdSpec<T> : Specification<ClientGoal, T>
{
    public ClientGoalByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

public class CurrentClientGoalByIdSpec<T> : Specification<ClientGoal, T>
{
    public CurrentClientGoalByIdSpec(DefaultIdType clientId)
    {
        Query.Where(p => p.ClientId == clientId).OrderByDescending(x => x.CreatedOn);
    }
}
