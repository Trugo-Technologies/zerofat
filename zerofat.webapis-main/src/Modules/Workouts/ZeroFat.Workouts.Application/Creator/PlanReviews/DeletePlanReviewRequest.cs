using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.PlanReviews;

public class DeletePlanReviewRequest(DefaultIdType id) : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class DeletePlanReviewRequestHandler(IRepositoryWithEvents<PlanReview> repository, IStringLocalizer<DeletePlanReviewRequestHandler> localizer) : IRequestHandler<DeletePlanReviewRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<PlanReview> _repository = repository;
    private readonly IStringLocalizer<DeletePlanReviewRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeletePlanReviewRequest request, CancellationToken cancellationToken)
    {
        var review = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = review ?? throw new NotFoundException(_localizer["Review not found"]);

        if (review.TotalRate != 0)
        {
            review.Content = null;
            await _repository.UpdateAsync(review, cancellationToken);
        } else
        {
            await _repository.DeleteAsync(review, cancellationToken);
        }

        return await Result<DefaultIdType>.SuccessAsync(review.Id);
    }

}
