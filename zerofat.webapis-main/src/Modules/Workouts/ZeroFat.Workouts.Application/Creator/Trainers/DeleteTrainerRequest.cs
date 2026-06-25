using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.Trainers;

public class DeleteTrainerRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteTrainerRequest(DefaultIdType id) => Id = id;
}


public class DeleteTrainerRequestHandler(IRepository<Trainer> repository, IStringLocalizer<DeleteTrainerRequestHandler> localizer, IReadRepository<Plan> planRepo, IReadRepository<Exercise> exerciseRepo, IReadRepository<Workout> workoutRepo) : IRequestHandler<DeleteTrainerRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Trainer> _repository = repository;
    private readonly IReadRepository<Plan> _planRepo = planRepo;
    private readonly IReadRepository<Workout> _workoutRepo = workoutRepo;
    private readonly IReadRepository<Exercise> _exerciseRepo = exerciseRepo;
    private readonly IStringLocalizer<DeleteTrainerRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteTrainerRequest request, CancellationToken cancellationToken)
    {
        var trainer = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = trainer ?? throw new NotFoundException(_localizer["Trainer not found"]);

        bool used = await _planRepo.AnyAsync(new ExpressionSpecification<Plan>(x => x.TrainerId == trainer.Id), cancellationToken);
        if (used)
            throw new BadRequestException(_localizer["Can not be deleted, because the trainer has plans"]);

        used = await _exerciseRepo.AnyAsync(new ExpressionSpecification<Exercise>(x => x.TrainerId == trainer.Id), cancellationToken);
        if (used)
            throw new BadRequestException(_localizer["Can not be deleted, because the trainer has exercises"]);

        used = await _workoutRepo.AnyAsync(new ExpressionSpecification<Workout>(x => x.TrainerId == trainer.Id), cancellationToken);
        if (used)
            throw new BadRequestException(_localizer["Can not be deleted, because the trainer has workouts"]);

        trainer.SpecialisesIn.Clear();

        await _repository.DeleteAsync(trainer, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(trainer.Id);
    }

}
