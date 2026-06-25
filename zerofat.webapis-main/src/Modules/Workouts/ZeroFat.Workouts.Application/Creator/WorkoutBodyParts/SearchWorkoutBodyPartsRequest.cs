using MediatR;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.WorkoutBodyParts;
public class SearchWorkoutBodyPartsRequest : PaginationFilter, IQuery<PaginationResponse<WorkoutBodyPartSimplifyDto>>
{
    public DefaultIdType? WorkoutId { get; set; }
    public DefaultIdType? BodyPartId { get; set; }
}


public class SearchWorkoutBodyPartsRequestHandler(IReadRepository<WorkoutBodyPart> repository) : IRequestHandler<SearchWorkoutBodyPartsRequest, PaginationResponse<WorkoutBodyPartSimplifyDto>>
{
    private readonly IReadRepository<WorkoutBodyPart> _repository = repository;

    public async Task<PaginationResponse<WorkoutBodyPartSimplifyDto>> Handle(SearchWorkoutBodyPartsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new WorkoutBodyPartsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
