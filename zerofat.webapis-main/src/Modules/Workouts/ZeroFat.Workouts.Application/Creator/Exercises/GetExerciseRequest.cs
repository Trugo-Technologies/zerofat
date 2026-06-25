using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.Exercises;
public class GetExerciseRequest(DefaultIdType id) : IQuery<Result<ExerciseDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetExerciseRequestHandler(IReadRepository<Exercise> repository, IStringLocalizer<GetExerciseRequestHandler> localizer) : IRequestHandler<GetExerciseRequest, Result<ExerciseDetailsDto>>
{
    private readonly IReadRepository<Exercise> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<ExerciseDetailsDto>> Handle(GetExerciseRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<Exercise, ExerciseDetailsDto>()
                .Map(destination => destination.BodyParts, src => src.ExerciseBodyParts.Select(x => x.BodyPart));


        var entity = await _repository.FirstOrDefaultAsync(new ExerciseByIdSpec<ExerciseDetailsDto>(request.Id), config, cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Exercise not found", request.Id]);

        entity.BodyPartIds = entity.BodyParts?.Select(x => x.Id).ToList();
        return await Result<ExerciseDetailsDto>.SuccessAsync(entity);
    }

}
