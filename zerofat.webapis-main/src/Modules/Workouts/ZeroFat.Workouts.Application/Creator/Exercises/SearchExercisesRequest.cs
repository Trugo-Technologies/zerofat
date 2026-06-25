using Mapster;
using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Application.Creator.Exercises;
public class SearchExercisesRequest : PaginationFilter, IQuery<PaginationResponse<ExerciseDto>>
{
    public bool? IsActive { get; set; }
    public DefaultIdType? TrainerId { get; set; }
    public DefaultIdType? BodyPartId { get; set; }
    public ExerciseType? Type { get; set; }

}


public class SearchExercisesRequestHandler(IReadRepository<Exercise> repository) : IRequestHandler<SearchExercisesRequest, PaginationResponse<ExerciseDto>>
{
    private readonly IReadRepository<Exercise> _repository = repository;

    public async Task<PaginationResponse<ExerciseDto>> Handle(SearchExercisesRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<Exercise, ExerciseDto>()
                .Map(destination => destination.BodyParts, src => src.ExerciseBodyParts.Select(x => x.BodyPart));

        return await _repository.PaginatedListAsync(new ExercisesBySearchRequestSpec(request), request.PageNumber, request.PageSize, config, cancellationToken);
    }
}
