using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Core.PhysicalActivityLevels;
using ZeroFat.ClientPortal.Domain.NutritionTracking;
using ZeroFat.Domain.Core;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientGoals;

public class GetCurrentClientGoalRequest : IQuery<Result<ClientGoalDetailsDto>>
{
}

public class GetCurrentClientGoalRequestHandler(
    IRepositoryWithEvents<ClientGoal> repository,
    IRepositoryWithEvents<PhysicalActivityLevel> palRepo,
    ICurrentUser currentUser,
    IStringLocalizer<GetCurrentClientGoalRequestHandler> localizer) : IRequestHandler<GetCurrentClientGoalRequest, Result<ClientGoalDetailsDto>>
{
    private readonly IRepositoryWithEvents<ClientGoal> _repository = repository;
    private readonly IRepositoryWithEvents<PhysicalActivityLevel> _palRepo = palRepo;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<ClientGoalDetailsDto>> Handle(GetCurrentClientGoalRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new CurrentClientGoalByIdSpec<ClientGoalDetailsDto>(_currentUser.GetUserId()), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Goal for current not found"]);

        entity.PhysicalActivityLevel = await _palRepo.FirstOrDefaultAsync(new PhysicalActivityLevelByIdSpec<PhysicalActivityLevelSimplifyDto>(entity.PhysicalActivityLevelId), cancellationToken);
        return await Result<ClientGoalDetailsDto>.SuccessAsync(entity);
    }
}
