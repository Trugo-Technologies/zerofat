using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Application.Shared;
using ZeroFat.Domain.Common;

namespace ZeroFat.GymUp.Application.Creator.ClientWorkouts;

public class CreateClientWorkoutRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType WorkoutId { get; set; }
    public double Calories { get; set; }
}

public class CreateClientWorkoutRequestValidator : CustomValidator<CreateClientWorkoutRequest>
{
    public CreateClientWorkoutRequestValidator(IReadRepository<Workout> repository, IStringLocalizer<CreateClientWorkoutRequestValidator> localaizer)
    {

        RuleFor(u => u.WorkoutId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await repository.AnyAsync(new ExpressionSpecification<Workout>(x => x.Id == id), _))
                .WithMessage(localaizer["Workout not found"]);
    }
}


public class CreateClientWorkoutRequestHandler(
    IRepositoryWithEvents<ClientWorkout> repository,
    IClientService clientService,
    ICurrentUser currentUser) : IRequestHandler<CreateClientWorkoutRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<ClientWorkout> _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IClientService _clientService = clientService;

    public async Task<Result<DefaultIdType>> Handle(CreateClientWorkoutRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var date = DateOnly.FromDateTime(SystemTime.Now());
        var clientWorkout = await _repository.FirstOrDefaultAsync(new ExpressionSpecification<ClientWorkout>(x => x.WorkoutId == request.WorkoutId && x.UserId == userId && x.Date == date), cancellationToken);
        if (clientWorkout is not null)
        {
            clientWorkout.Calories = request.Calories;
            await _repository.UpdateAsync(clientWorkout, cancellationToken);
        }
        else
        {
            clientWorkout = new ClientWorkout
            {
                WorkoutId = request.WorkoutId,
                UserId = userId,
                Calories = request.Calories,
                Date = date
            };

            await _clientService.UpdateClientStatisticesFromWorkout(date, userId, request.Calories, "Workout");
            await _repository.AddAsync(clientWorkout, cancellationToken: cancellationToken);
        }

        return await Result<DefaultIdType>.SuccessAsync(clientWorkout.Id);
    }

}
