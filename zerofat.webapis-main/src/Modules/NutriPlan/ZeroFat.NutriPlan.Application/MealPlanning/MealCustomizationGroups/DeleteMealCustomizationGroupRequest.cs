using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationGroups;

public class DeleteMealCustomizationGroupRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteMealCustomizationGroupRequest(DefaultIdType id) => Id = id;
}


public class DeleteMealCustomizationGroupRequestHandler(IRepository<MealCustomizationGroup> repository, IStringLocalizer<DeleteMealCustomizationGroupRequestHandler> localizer) : IRequestHandler<DeleteMealCustomizationGroupRequest, Result<DefaultIdType>>
{
    private readonly IRepository<MealCustomizationGroup> _repository = repository;
    private readonly IStringLocalizer<DeleteMealCustomizationGroupRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteMealCustomizationGroupRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["MealCustomizationGroup not found"]);

        await _repository.DeleteAsync(part, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
