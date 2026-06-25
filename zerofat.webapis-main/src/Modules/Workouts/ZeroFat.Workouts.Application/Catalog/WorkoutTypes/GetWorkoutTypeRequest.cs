using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Catalog.WorkoutTypes;
public class GetWorkoutTypeRequest(DefaultIdType id) : IQuery<Result<WorkoutTypeDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetWorkoutTypesRequestHandler(IRepositoryWithEvents<WorkoutType> repository, IStringLocalizer<GetWorkoutTypesRequestHandler> localizer) : IRequestHandler<GetWorkoutTypeRequest, Result<WorkoutTypeDetailsDto>>
{
    private readonly IRepositoryWithEvents<WorkoutType> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<WorkoutTypeDetailsDto>> Handle(GetWorkoutTypeRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new WorkoutTypeByIdSpec<WorkoutTypeDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["WorkoutTypes not found", request.Id]);

        return await Result<WorkoutTypeDetailsDto>.SuccessAsync(entity);
    }

}
