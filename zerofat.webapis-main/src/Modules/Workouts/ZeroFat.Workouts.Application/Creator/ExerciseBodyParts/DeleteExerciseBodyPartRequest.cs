using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.ExerciseBodyParts;

public class DeleteExerciseBodyPartRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteExerciseBodyPartRequest(DefaultIdType id)
    {
        Id = id;
    }
}


public class DeleteExerciseBodyPartRequestHandler(IRepositoryWithEvents<ExerciseBodyPart> repository, IStringLocalizer<DeleteExerciseBodyPartRequestHandler> localizer) : IRequestHandler<DeleteExerciseBodyPartRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<ExerciseBodyPart> _repository = repository;
    private readonly IStringLocalizer<DeleteExerciseBodyPartRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteExerciseBodyPartRequest request, CancellationToken cancellationToken)
    {
        var exerciseBodyPart = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = exerciseBodyPart ?? throw new NotFoundException(_localizer["Exercise body part not found"]);

        await _repository.DeleteAsync(exerciseBodyPart, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(exerciseBodyPart.Id);
    }

}
