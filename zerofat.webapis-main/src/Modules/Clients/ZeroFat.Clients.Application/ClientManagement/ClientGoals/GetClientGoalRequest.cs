using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.NutritionTracking;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientGoals;

public class GetClientGoalRequest(DefaultIdType id) : IQuery<Result<ClientGoalDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetClientGoalRequestHandler(IRepositoryWithEvents<ClientGoal> repository, IStringLocalizer<GetClientGoalRequestHandler> localizer) : IRequestHandler<GetClientGoalRequest, Result<ClientGoalDetailsDto>>
{
    private readonly IRepositoryWithEvents<ClientGoal> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<ClientGoalDetailsDto>> Handle(GetClientGoalRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new ClientGoalByIdSpec<ClientGoalDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Location not found", request.Id]);

        return await Result<ClientGoalDetailsDto>.SuccessAsync(entity);
    }
}
