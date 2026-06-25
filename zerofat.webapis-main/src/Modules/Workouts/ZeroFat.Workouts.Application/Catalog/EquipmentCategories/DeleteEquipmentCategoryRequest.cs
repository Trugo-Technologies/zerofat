using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.EquipmentCategories;

public class DeleteEquipmentCategoryRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteEquipmentCategoryRequest(DefaultIdType id)
    {
        Id = id;
    }
}


public class DeleteEquipmentCategoryRequestHandler(IRepository<EquipmentCategory> repository, IStringLocalizer<DeleteEquipmentCategoryRequestHandler> localizer, IReadRepository<Equipment> equipmentRepo) : IRequestHandler<DeleteEquipmentCategoryRequest, Result<DefaultIdType>>
{
    private readonly IRepository<EquipmentCategory> _repository = repository;
    private readonly IReadRepository<Equipment> _equipmentRepo = equipmentRepo;
    private readonly IStringLocalizer<DeleteEquipmentCategoryRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteEquipmentCategoryRequest request, CancellationToken cancellationToken)
    {
        var cate = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = cate ?? throw new NotFoundException(_localizer["Category not found"]);

        bool used = await _equipmentRepo.AnyAsync(new ExpressionSpecification<Equipment>(x => x.CategoryId == cate.Id), cancellationToken);
        if (used)
            throw new BadRequestException(_localizer["Can not be deleted, because it's linked with equipmnts"]);

        await _repository.DeleteAsync(cate, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(cate.Id);
    }

}
