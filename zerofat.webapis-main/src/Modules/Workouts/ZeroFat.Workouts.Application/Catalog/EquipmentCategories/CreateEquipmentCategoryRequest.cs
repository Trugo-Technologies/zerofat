using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Domain;

namespace ZeroFat.GymUp.Application.Catalog.EquipmentCategories;
public class CreateEquipmentCategoryRequest : ICommand<Result<DefaultIdType>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? Icon { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateEquipmentCategoryRequestValidator : CustomValidator<CreateEquipmentCategoryRequest>
{
    public CreateEquipmentCategoryRequestValidator(IReadRepository<EquipmentCategory> repository, IStringLocalizer<CreateEquipmentCategoryRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<EquipmentCategory>(x => x.NameAr == name), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<EquipmentCategory>(x => x.NameEn == name), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}


public class CreateEquipmentCategoryRequestHandler(IRepositoryWithEvents<EquipmentCategory> repository, IFileStorageManager uploadFile) : IRequestHandler<CreateEquipmentCategoryRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<EquipmentCategory> _repository = repository;
    private readonly IFileStorageManager _uploadFile = uploadFile;

    public async Task<Result<DefaultIdType>> Handle(CreateEquipmentCategoryRequest request, CancellationToken cancellationToken)
    {
        var cate = new EquipmentCategory
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            IconUrl = await _uploadFile.UploadAsync<EquipmentCategory>(request.Icon, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
            IsActive = request.IsActive ?? false
        };

        await _repository.AddAsync(cate, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(cate.Id);
    }

}
