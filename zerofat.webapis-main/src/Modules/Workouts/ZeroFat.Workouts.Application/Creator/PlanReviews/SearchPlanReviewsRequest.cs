
using MediatR;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.PlanReviews;
public class SearchPlanReviewsRequest : PaginationFilter, IQuery<PaginationResponse<PlanReviewSimplifyDto>>
{
    public DefaultIdType? PlanId { get; set; }
    public DefaultIdType? UserId { get; set; }
}

public class SearchPlanReviewsRequestHandler(IReadRepository<PlanReview> repository) : IRequestHandler<SearchPlanReviewsRequest, PaginationResponse<PlanReviewSimplifyDto>>
{
    private readonly IReadRepository<PlanReview> _repository = repository;

    public async Task<PaginationResponse<PlanReviewSimplifyDto>> Handle(SearchPlanReviewsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new PlanReviewBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
