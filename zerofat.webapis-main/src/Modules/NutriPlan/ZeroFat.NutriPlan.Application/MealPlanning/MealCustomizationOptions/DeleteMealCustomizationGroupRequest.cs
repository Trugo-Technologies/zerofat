using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationOptions;

public class DeleteMealCustomizationOptionRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteMealCustomizationOptionRequest(DefaultIdType id) => Id = id;
}


public class DeleteMealCustomizationOptionRequestHandler(IRepository<MealCustomizationOption> repository, IStringLocalizer<DeleteMealCustomizationOptionRequestHandler> localizer) : IRequestHandler<DeleteMealCustomizationOptionRequest, Result<DefaultIdType>>
{
    private readonly IRepository<MealCustomizationOption> _repository = repository;
    private readonly IStringLocalizer<DeleteMealCustomizationOptionRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteMealCustomizationOptionRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["MealCustomizationOption not found"]);

        await _repository.DeleteAsync(part, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
