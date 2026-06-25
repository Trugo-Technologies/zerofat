using MediatR;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.ClientWorkouts;
public class SearchClientWorkoutsRequest : PaginationFilter, IQuery<PaginationResponse<ClientWorkoutDetailsDto>>
{
    public DefaultIdType? WorkoutId { get; set; }
    public DefaultIdType? UserId { get; set; }
}


public class SearchClientWorkoutsRequestHandler(IReadRepository<ClientWorkout> repository) : IRequestHandler<SearchClientWorkoutsRequest, PaginationResponse<ClientWorkoutDetailsDto>>
{
    private readonly IReadRepository<ClientWorkout> _repository = repository;

    public async Task<PaginationResponse<ClientWorkoutDetailsDto>> Handle(SearchClientWorkoutsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new ClientWorkoutsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
