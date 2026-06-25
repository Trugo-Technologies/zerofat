using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.GymUp.Application.Creator.PlanSchedules;

namespace ZeroFat.GymUp.Application.Creator.Plans;
public class GetMobilePlanRequest : IQuery<Result<PlanMobileDetailsDto>>
{
    public DefaultIdType Id { get; set; }
    public bool? WithSchedule { get; set; }
    public GetMobilePlanRequest(Guid id)
    {
        Id = id;
    }
}

public class GetMobilePlanRequestHandler(IReadRepository<Plan> repository, ICurrentUser currentUser, IStringLocalizer<GetMobilePlanRequestHandler> localizer, IReadRepository<PlanSchedule> scheduleRepo) : IRequestHandler<GetMobilePlanRequest, Result<PlanMobileDetailsDto>>
{
    private readonly IReadRepository<Plan> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<PlanMobileDetailsDto>> Handle(GetMobilePlanRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<Plan, PlanMobileDetailsDto>()
                .Map(destination => destination.TotalRate, src => src.PlanReviews.Sum(x => x.TotalRate))
                .Map(destination => destination.EasyToUseRate, src => src.PlanReviews.Sum(x => x.EasyToUseRate))
                .Map(destination => destination.EnjoyabilityRate, src => src.PlanReviews.Sum(x => x.EnjoyabilityRate))
                .Map(destination => destination.Schedules, src => src.Schedules.Where(x => x.Workout.IsActive))
                .Map(destination => destination.EffectivenessRate, src => src.PlanReviews.Sum(x => x.EffectivenessRate))
                .Map(destination => destination.PlanReviews, src => src.PlanReviews.OrderByDescending(x => x.CreatedOn).Take(5))
                .Map(destination => destination.IsWishlist, src => src.PlanWishlists.Any(x => x.UserId == _currentUser.GetUserId()));

        var entity = await _repository.FirstOrDefaultAsync(new PlanByIdSpec<PlanMobileDetailsDto>(request.Id), config, cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Plan not found", request.Id]);

        return await Result<PlanMobileDetailsDto>.SuccessAsync(entity);
    }

}
