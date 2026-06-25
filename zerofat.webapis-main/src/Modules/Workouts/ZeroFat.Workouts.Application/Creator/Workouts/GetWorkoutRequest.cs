using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Common;
using ZeroFat.GymUp.Application.Creator.ClientWorkouts;

namespace ZeroFat.GymUp.Application.Creator.Workouts;
public class GetWorkoutRequest(DefaultIdType id) : IQuery<Result<WorkoutDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetWorkoutRequestHandler(IReadRepository<Workout> repository, IStringLocalizer<GetWorkoutRequestHandler> localizer, ICurrentUser currentUser, IReadRepository<ClientWorkout> clientWorkoutRepo) : IRequestHandler<GetWorkoutRequest, Result<WorkoutDetailsDto>>
{
    private readonly IReadRepository<Workout> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly IReadRepository<ClientWorkout> _clientWorkoutRepo = clientWorkoutRepo;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<WorkoutDetailsDto>> Handle(GetWorkoutRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<Workout, WorkoutDetailsDto>()
                .Map(destination => destination.BodyParts, src => src.WorkoutBodyParts.Select(x => x.BodyPart))
                .Map(destination => destination.Equipments, src => src.WorkoutEquipments.Select(x => x.Equipment));

        var entity = await _repository.FirstOrDefaultAsync(new WorkoutByIdSpec<WorkoutDetailsDto>(request.Id), config, cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Workout not found", request.Id]);

        entity.BodyPartIds = entity.BodyParts?.Select(x => x.Id).ToList();
        entity.EquipmentIds = entity.Equipments?.Select(x => x.Id).ToList();

        if (_currentUser.GetUserId() != Guid.Empty)
        {
            var date = DateOnly.FromDateTime(SystemTime.Now());
            entity.ClientWorkout = await _clientWorkoutRepo.FirstOrDefaultAsync(new ClientWorkoutBySpec<ClientWorkoutSimplifyDto>(_currentUser.GetUserId(), entity.Id, date), cancellationToken);
        }
        return await Result<WorkoutDetailsDto>.SuccessAsync(entity);
    }

}
