using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.PlanReviews;
public class UpsertPlanReviewRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType PlanId { get; set; }
    public double? EffectivenessRate { get; set; }
    public double? EasyToUseRate { get; set; }
    public double? EnjoyabilityRate { get; set; }
    public double? TotalRate { get; set; }
    public string? Content { get; set; }
}

public class CreatePlanReviewRequestValidator : AbstractValidator<UpsertPlanReviewRequest>
{
    public CreatePlanReviewRequestValidator(IReadRepository<PlanReview> repository, IReadRepository<Plan> planRepo, IStringLocalizer<CreatePlanReviewRequestValidator> localaizer)
    {

        RuleFor(u => u.PlanId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await planRepo.AnyAsync(new ExpressionSpecification<Plan>(x => x.Id == id), _))
                .WithMessage(localaizer["Plan not found"]);
    }
}

public class CreatePlanReviewRequestHandler(IRepository<PlanReview> repo, IStringLocalizer<CreatePlanReviewRequestHandler> localizer, ICurrentUser currentUser) : IRequestHandler<UpsertPlanReviewRequest, Result<DefaultIdType>>
{
    private IRepository<PlanReview> _repo = repo;
    private ICurrentUser _currentUser = currentUser;
    private IStringLocalizer<CreatePlanReviewRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(UpsertPlanReviewRequest request, CancellationToken cancellationToken)
    {
        if (!request.Content.HasValue() && !(request.EasyToUseRate.HasValue || request.EnjoyabilityRate.HasValue || request.EffectivenessRate.HasValue))
            throw new BadRequestException(_localizer["Missing info"]);

        var exists = await _repo.FirstOrDefaultAsync(new ExpressionSpecification<PlanReview>(x => x.PlanId == request.PlanId && x.UserId == _currentUser.GetUserId()), cancellationToken);

        if (exists is not null)
        {
            if (request.Content.HasValue())
                exists.Content = request.Content;
            if (request.EffectivenessRate.HasValue)
                exists.EffectivenessRate = request.EffectivenessRate!.Value;
            if (request.EasyToUseRate.HasValue)
                exists.EasyToUseRate = request.EasyToUseRate!.Value;
            if (request.EnjoyabilityRate.HasValue)
                exists.EnjoyabilityRate = request.EnjoyabilityRate!.Value;

            exists.TotalRate = (exists.EasyToUseRate + exists.EnjoyabilityRate + exists.EffectivenessRate) / 3;
            await _repo.UpdateAsync(exists, cancellationToken);
            return await Result<DefaultIdType>.SuccessAsync(exists.Id);
        }
        else
        {

            exists = new PlanReview
            {
                PlanId = request.PlanId,
                UserId = _currentUser.GetUserId(),
                Content = request.Content,
                EffectivenessRate = request.EffectivenessRate ?? 0,
                EasyToUseRate = request.EasyToUseRate ?? 0,
                EnjoyabilityRate = request.EnjoyabilityRate ?? 0,
            };

            exists.TotalRate = (exists.EasyToUseRate + exists.EnjoyabilityRate + exists.EffectivenessRate) / 3;
        }

        await _repo.AddAsync(exists, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(exists.Id);
    }
}
