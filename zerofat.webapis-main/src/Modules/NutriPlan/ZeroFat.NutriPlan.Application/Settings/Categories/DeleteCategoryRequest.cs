using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.Categories;

public class DeleteCategoryRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteCategoryRequest(DefaultIdType id) => Id = id;
}


public class DeleteCategoryRequestHandler(IRepository<Category> repository, IStringLocalizer<DeleteCategoryRequestHandler> localizer) : IRequestHandler<DeleteCategoryRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Category> _repository = repository;
    private readonly IStringLocalizer<DeleteCategoryRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["Category not found"]);

        await _repository.DeleteAsync(part, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
