using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.Allergens;
public class GetAllergenRequest(DefaultIdType id) : IQuery<Result<AllergenDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetAllergenRequestHandler(IRepositoryWithEvents<Allergen> repository, IStringLocalizer<GetAllergenRequestHandler> localizer) : IRequestHandler<GetAllergenRequest, Result<AllergenDetailsDto>>
{
    private readonly IRepositoryWithEvents<Allergen> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<AllergenDetailsDto>> Handle(GetAllergenRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new AllergenByIdSpec<AllergenDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Allergen not found", request.Id]);

        return await Result<AllergenDetailsDto>.SuccessAsync(entity);
    }
}
