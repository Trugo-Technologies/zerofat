using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Common;
using ZeroFat.GymUp.Application.Creator.ClientWorkouts;

namespace ZeroFat.GymUp.Application.Creator.Workouts;
public class GetWorkoutWithExercisesRequest(DefaultIdType id) : IQuery<Result<WorkoutMobileDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetWorkoutWithExercisesRequestHandler(IReadRepository<Workout> repository, IStringLocalizer<GetWorkoutWithExercisesRequestHandler> localizer, IReadRepository<ClientWorkout> clientWorkoutRepo, ICurrentUser currentUser) : IRequestHandler<GetWorkoutWithExercisesRequest, Result<WorkoutMobileDetailsDto>>
{
    private readonly IReadRepository<Workout> _repository = repository;
    private readonly IReadRepository<ClientWorkout> _clientWorkoutRepo = clientWorkoutRepo;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<WorkoutMobileDetailsDto>> Handle(GetWorkoutWithExercisesRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<Workout, WorkoutMobileDetailsDto>()
                .Map(destination => destination.Equipments, src => src.WorkoutEquipments.Select(x => x.Equipment));

        var entity = await _repository.FirstOrDefaultAsync(new WorkoutByIdSpec<WorkoutMobileDetailsDto>(request.Id), config, cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Workout not found", request.Id]);

        if (_currentUser.GetUserId() != Guid.Empty)
        {
            var date = DateOnly.FromDateTime(SystemTime.Now());
            entity.ClientWorkout = await _clientWorkoutRepo.FirstOrDefaultAsync(new ClientWorkoutBySpec<ClientWorkoutSimplifyDto>(_currentUser.GetUserId(), entity.Id, date), cancellationToken);
        }
        return await Result<WorkoutMobileDetailsDto>.SuccessAsync(entity);
    }

}
