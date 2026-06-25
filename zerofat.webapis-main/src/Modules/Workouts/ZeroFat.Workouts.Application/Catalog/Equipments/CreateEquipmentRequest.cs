using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Domain;

namespace ZeroFat.GymUp.Application.Catalog.Equipments;
public class CreateEquipmentRequest : ICommand<Result<DefaultIdType>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public DefaultIdType CategoryId { get; set; }
    public IFormFile? Image { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateEquipmentRequestValidator : CustomValidator<CreateEquipmentRequest>
{
    public CreateEquipmentRequestValidator(IReadRepository<Equipment> repository, IReadRepository<EquipmentCategory> eqCatRepo, IStringLocalizer<CreateEquipmentRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<Equipment>(x => x.NameAr == name), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<Equipment>(x => x.NameEn == name), _))
                .WithMessage(localaizer["English name already exists"]);

        RuleFor(u => u.CategoryId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await eqCatRepo.AnyAsync(new ExpressionSpecification<EquipmentCategory>(x => x.Id == id), _))
                .WithMessage(localaizer["Category not found"]);

    }
}


public class CreateEquipmentRequestHandler(
    IRepositoryWithEvents<Equipment> repository,
    IFileStorageManager uploadFile) : IRequestHandler<CreateEquipmentRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Equipment> _repository = repository;
    private readonly IFileStorageManager _uploadFile = uploadFile;

    public async Task<Result<DefaultIdType>> Handle(CreateEquipmentRequest request, CancellationToken cancellationToken)
    {
        var part = new Equipment
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            CategoryId = request.CategoryId,
            IsActive = request.IsActive ?? false,
        };

        if (request.Image != null)
        {
            part.ImageUrl = await _uploadFile.UploadAsync<Equipment>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            part.ImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Equipment>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            part.ImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Equipment>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        await _repository.AddAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
