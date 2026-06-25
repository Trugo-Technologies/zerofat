using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Extras;

public class DeleteExtraRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteExtraRequest(DefaultIdType id) => Id = id;
}


public class DeleteExtraRequestHandler(IRepository<Extra> repository, IStringLocalizer<DeleteExtraRequestHandler> localizer) : IRequestHandler<DeleteExtraRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Extra> _repository = repository;
    private readonly IStringLocalizer<DeleteExtraRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteExtraRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["Extra not found"]);

        await _repository.DeleteAsync(part, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
