using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.NutrientsAttributes;
public class GetNutrientsAttributeRequest(Guid id) : IQuery<Result<NutrientsAttributeDetailsDto>>
{
    public Guid Id { get; set; } = id;
}

public class GetNutrientsAttributeRequestHandler(IRepositoryWithEvents<NutrientsAttribute> repository, IStringLocalizer<GetNutrientsAttributeRequestHandler> localizer) : IRequestHandler<GetNutrientsAttributeRequest, Result<NutrientsAttributeDetailsDto>>
{
    private readonly IRepositoryWithEvents<NutrientsAttribute> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<NutrientsAttributeDetailsDto>> Handle(GetNutrientsAttributeRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new NutrientsAttributeByIdSpec<NutrientsAttributeDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["NutrientsAttribute not found", request.Id]);

        return await Result<NutrientsAttributeDetailsDto>.SuccessAsync(entity);
    }
}
