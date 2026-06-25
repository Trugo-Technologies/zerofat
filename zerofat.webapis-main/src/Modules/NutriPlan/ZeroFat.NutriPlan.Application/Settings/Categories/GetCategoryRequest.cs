using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.Categories;
public class GetCategoryRequest(DefaultIdType id) : IQuery<Result<CategoryDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetCategoryRequestHandler(IRepositoryWithEvents<Category> repository, IStringLocalizer<GetCategoryRequestHandler> localizer) : IRequestHandler<GetCategoryRequest, Result<CategoryDetailsDto>>
{
    private readonly IRepositoryWithEvents<Category> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<CategoryDetailsDto>> Handle(GetCategoryRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new CategoryByIdSpec<CategoryDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Category not found", request.Id]);

        return await Result<CategoryDetailsDto>.SuccessAsync(entity);
    }
}
