using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.BodyParts;
public class DeleteBodyPartsRequest : ICommand<Result<bool>>
{
    public List<DefaultIdType> Ids { get; set; } = [];
}

public class DeleteBodyPartsRequestHandler(
    IRepositoryWithEvents<BodyPart> repository,
    IStringLocalizer<DeleteBodyPartRequestHandler> localizer,
    IReadRepository<WorkoutBodyPart> workoutBodyPartRepo,
    IReadRepository<ExerciseBodyPart> exerciseBodyPartRepo) : IRequestHandler<DeleteBodyPartsRequest, Result<bool>>
{

    public async Task<Result<bool>> Handle(DeleteBodyPartsRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var bodyPart = await repository.GetByIdAsync(ingId, cancellationToken);
            if (bodyPart != null)
            {
                if (await workoutBodyPartRepo.AnyAsync(new ExpressionSpecification<WorkoutBodyPart>(x => x.BodyPartId == bodyPart.Id), cancellationToken))
                    continue;

                if (await exerciseBodyPartRepo.AnyAsync(new ExpressionSpecification<ExerciseBodyPart>(x => x.BodyPartId == bodyPart.Id), cancellationToken))
                    continue;

                await repository.DeleteAsync(bodyPart, withSaveChanges: false, cancellationToken: cancellationToken);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return await Result<bool>.SuccessAsync(data :true);
    }

}
