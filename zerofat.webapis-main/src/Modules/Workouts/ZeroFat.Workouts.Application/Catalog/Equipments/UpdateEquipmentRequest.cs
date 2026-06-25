using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Domain;
using ZeroFat.GymUp.Domain.Catalog;

namespace ZeroFat.GymUp.Application.Catalog.Equipments;
public class UpdateEquipmentRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public DefaultIdType CategoryId { get; set; }
    public bool IsActive { get; set; }
    public IFormFile? Image { get; set; }
}

public class UpdateEquipmentRequestValidator : CustomValidator<UpdateEquipmentRequest>
{
    public UpdateEquipmentRequestValidator(IReadRepository<Equipment> repository, IReadRepository<EquipmentCategory> eqCatRepo, IStringLocalizer<UpdateEquipmentRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Equipment>(x => x.NameAr == name && req.Id != x.Id), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Equipment>(x => x.NameEn == name && req.Id != x.Id), _))
                .WithMessage(localaizer["English name already exists"]);

        RuleFor(u => u.CategoryId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await eqCatRepo.AnyAsync(new ExpressionSpecification<EquipmentCategory>(x => x.Id == id), _))
                .WithMessage(localaizer["Category not found"]);

    }
}

public class UpdateEquipmentRequestHandler(
    IRepositoryWithEvents<Equipment> repository,
    IFileStorageManager uploadFile,
    IStringLocalizer<UpdateEquipmentRequestHandler> localizer) : IRequestHandler<UpdateEquipmentRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Equipment> _repository = repository;
    private readonly IStringLocalizer<UpdateEquipmentRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _uploadFile = uploadFile;

    public async Task<Result<DefaultIdType>> Handle(UpdateEquipmentRequest request, CancellationToken cancellationToken)
    {
        var eq = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = eq ?? throw new NotFoundException(_localizer["Body part not found"]);

        eq.NameAr = request.NameAr;
        eq.NameEn = request.NameEn;
        eq.CategoryId = request.CategoryId;
        eq.IsActive = request.IsActive;

        if (request.Image != null)
        {
            eq.ImageUrl = await _uploadFile.UploadAsync<Equipment>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            eq.ImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Equipment>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            eq.ImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Equipment>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        await _repository.UpdateAsync(eq, withSaveChanges: true, cancellationToken: cancellationToken);
        return await Result<DefaultIdType>.SuccessAsync(eq.Id);
    }
}

