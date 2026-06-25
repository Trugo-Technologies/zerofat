using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.Trainers;
public class GetTrainerRequest(DefaultIdType id) : IQuery<Result<TrainerDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetTrainerRequestHandler(IRepositoryWithEvents<Trainer> repository, IStringLocalizer<GetTrainerRequestHandler> localizer) : IRequestHandler<GetTrainerRequest, Result<TrainerDetailsDto>>
{
    private readonly IRepositoryWithEvents<Trainer> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<TrainerDetailsDto>> Handle(GetTrainerRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new TrainerByIdSpec<TrainerDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Trainer not found", request.Id]);

        return await Result<TrainerDetailsDto>.SuccessAsync(entity);
    }

}
