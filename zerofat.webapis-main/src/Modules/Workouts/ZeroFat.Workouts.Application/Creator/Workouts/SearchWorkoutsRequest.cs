using Mapster;
using MediatR;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Common.Contracts;
using ZeroFat.Domain.Common;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Application.Creator.ClientWorkouts;

namespace ZeroFat.GymUp.Application.Creator.Workouts;
public class SearchWorkoutsRequest : PaginationFilter, IQuery<PaginationResponse<WorkoutDto>>
{
    public bool? IsActive { get; set; }
    public Level? Level { get; set; }
    public WorkoutFormat? Format { get; set; }
    public GymEnvironment? Environment { get; set; }
    public DefaultIdType? EquipmentCategoryId { get; set; }
    public DefaultIdType? TrainerId { get; set; }
    public DefaultIdType? WorkoutTypeId { get; set; }
    public DefaultIdType? BodyPartId { get; set; }
    public DefaultIdType? EquipmentId { get; set; }

}


public class SearchWorkoutsRequestHandler(IReadRepository<Workout> repository, IReadRepository<ClientWorkout> clientWorkoutRepo, ICurrentUser currentUser) : IRequestHandler<SearchWorkoutsRequest, PaginationResponse<WorkoutDto>>
{
    private readonly IReadRepository<Workout> _repository = repository;
    private readonly IReadRepository<ClientWorkout> _clientWorkoutRepo = clientWorkoutRepo;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<PaginationResponse<WorkoutDto>> Handle(SearchWorkoutsRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<Workout, WorkoutDto>()
                .Map(destination => destination.BodyParts, src => src.WorkoutBodyParts.Select(x => x.BodyPart))
                .Map(destination => destination.Equipments, src => src.WorkoutEquipments.Select(x => x.Equipment));

        var result = await _repository.PaginatedListAsync(new WorkoutsBySearchRequestSpec(request), request.PageNumber, request.PageSize, config, cancellationToken);
        if (_currentUser.GetUserId() != Guid.Empty)
        {
            var date = DateOnly.FromDateTime(SystemTime.Now());
            foreach (var res in result.Data)
                res.ClientWorkout = await _clientWorkoutRepo.FirstOrDefaultAsync(new ClientWorkoutBySpec<ClientWorkoutSimplifyDto>(_currentUser.GetUserId(), res.Id, date), cancellationToken);
        }

        return result;
    }

}
