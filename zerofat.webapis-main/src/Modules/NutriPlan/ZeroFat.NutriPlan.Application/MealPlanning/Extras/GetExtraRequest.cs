using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Extras;
public class GetExtraRequest(DefaultIdType id) : IQuery<Result<ExtraDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetExtraRequestHandler(IRepositoryWithEvents<Extra> repository, IStringLocalizer<GetExtraRequestHandler> localizer) : IRequestHandler<GetExtraRequest, Result<ExtraDetailsDto>>
{
    private readonly IRepositoryWithEvents<Extra> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<ExtraDetailsDto>> Handle(GetExtraRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new ExtraByIdSpec<ExtraDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Extra not found", request.Id]);

        return await Result<ExtraDetailsDto>.SuccessAsync(entity);
    }
}
