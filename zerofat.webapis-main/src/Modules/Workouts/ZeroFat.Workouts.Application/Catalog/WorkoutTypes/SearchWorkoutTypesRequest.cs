using MediatR;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Catalog.WorkoutTypes;
public class SearchWorkoutTypesRequest : PaginationFilter, IQuery<PaginationResponse<WorkoutTypeDto>>
{
    public bool? IsActive { get; set; }
}


public class SearchWorkoutTypesRequestHandler(IReadRepository<WorkoutType> repository) : IRequestHandler<SearchWorkoutTypesRequest, PaginationResponse<WorkoutTypeDto>>
{
    private readonly IReadRepository<WorkoutType> _repository = repository;

    public async Task<PaginationResponse<WorkoutTypeDto>> Handle(SearchWorkoutTypesRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new WorkoutTypesBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
