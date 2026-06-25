using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Application.Creator.Trainers;
public class SearchTrainersRequest : PaginationFilter, IQuery<PaginationResponse<TrainerDto>>
{
    public bool? IsActive { get; set; }
    public TrainerType? Type { get; set; }
}


public class SearchTrainersRequestHandler(IReadRepository<Trainer> repository) : IRequestHandler<SearchTrainersRequest, PaginationResponse<TrainerDto>>
{
    private readonly IReadRepository<Trainer> _repository = repository;

    public async Task<PaginationResponse<TrainerDto>> Handle(SearchTrainersRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new TrainersBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
