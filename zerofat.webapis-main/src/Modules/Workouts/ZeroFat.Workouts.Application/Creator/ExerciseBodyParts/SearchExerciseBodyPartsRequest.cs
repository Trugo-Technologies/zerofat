using MediatR;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.ExerciseBodyParts;
public class SearchExerciseBodyPartsRequest : PaginationFilter, IQuery<PaginationResponse<ExerciseBodyPartSimplifyDto>>
{
    public DefaultIdType? ExerciseId { get; set; }
    public DefaultIdType? BodyPartId { get; set; }
}


public class SearchExerciseBodyPartsRequestHandler(IReadRepository<ExerciseBodyPart> repository) : IRequestHandler<SearchExerciseBodyPartsRequest, PaginationResponse<ExerciseBodyPartSimplifyDto>>
{
    private readonly IReadRepository<ExerciseBodyPart> _repository = repository;

    public async Task<PaginationResponse<ExerciseBodyPartSimplifyDto>> Handle(SearchExerciseBodyPartsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new ExerciseBodyPartsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
