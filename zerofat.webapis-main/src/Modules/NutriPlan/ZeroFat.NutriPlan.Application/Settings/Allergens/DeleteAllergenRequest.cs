using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.Allergens;

public class DeleteAllergenRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteAllergenRequest(DefaultIdType id) => Id = id;
}


public class DeleteAllergenRequestHandler(IRepository<Allergen> repository, IStringLocalizer<DeleteAllergenRequestHandler> localizer) : IRequestHandler<DeleteAllergenRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Allergen> _repository = repository;
    private readonly IStringLocalizer<DeleteAllergenRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteAllergenRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["Allergen not found"]);

        await _repository.DeleteAsync(part, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
