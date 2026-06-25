using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Application.ClientManagement;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;

public class GetClientSubscriptionRequest(DefaultIdType id) : IQuery<Result<ClientSubscriptionDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetClientSubscriptionRequestHandler(
    IRepositoryWithEvents<ClientSubscription> repository, 
    IReadRepository<MealPlan> mealPlanRepo, 
    IReadRepository<ClientLocation> clientLocationRepo, 
    IStringLocalizer<GetClientSubscriptionRequestHandler> localizer) : IRequestHandler<GetClientSubscriptionRequest, Result<ClientSubscriptionDetailsDto>>
{
    private readonly IRepositoryWithEvents<ClientSubscription> _repository = repository;
    private readonly IReadRepository<MealPlan> _mealPlanRepo = mealPlanRepo;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<ClientSubscriptionDetailsDto>> Handle(GetClientSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new ClientSubscriptionByIdSpec<ClientSubscriptionDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["ClientSubscription not found", request.Id]);

        entity.MealPlan = await _mealPlanRepo.FirstOrDefaultAsync(new MealPlanByIdSpec<MealPlanDto>(entity.MealPlanId), cancellationToken);

        if(entity.ClientLocationId.HasValue)
            entity.ClientLocation = await clientLocationRepo.FirstOrDefaultAsync(new ClientLocationByIdSpec<ClientLocationSimplifyDto>(entity.ClientLocationId.Value), cancellationToken);

        return await Result<ClientSubscriptionDetailsDto>.SuccessAsync(entity);
    }

}
