using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.Trainers;
public class DeleteTrainersRequest : ICommand<Result<bool>>
{
    public List<DefaultIdType> Ids { get; set; } = [];
}

public class DeleteTrainersRequestHandler(
    IRepositoryWithEvents<Trainer> repository,
    IStringLocalizer<DeleteTrainersRequestHandler> localizer,
    IReadRepository<Plan> planRepo) : IRequestHandler<DeleteTrainersRequest, Result<bool>>
{

    public async Task<Result<bool>> Handle(DeleteTrainersRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var trainer = await repository.GetByIdAsync(ingId, cancellationToken);
            if (trainer != null)
            {
                if (await planRepo.AnyAsync(new ExpressionSpecification<Plan>(x => x.TrainerId == trainer.Id), cancellationToken))
                    continue;

                await repository.DeleteAsync(trainer, withSaveChanges: false, cancellationToken: cancellationToken);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return await Result<bool>.SuccessAsync(data: true);
    }

}
