using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MeasurementUnits;
public class UpdateMeasurementUnitRequest : ICommand<Result<Guid>>
{
    public Guid? Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string Code { get; set; } = default!;
    public IFormFile? Icon { get; set; }
}

public class UpdateMeasurementUnitRequestValidator : CustomValidator<UpdateMeasurementUnitRequest>
{
    public UpdateMeasurementUnitRequestValidator(IReadRepository<MeasurementUnit> repository, IStringLocalizer<UpdateMeasurementUnitRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<MeasurementUnit>(x => x.NameAr == name && req.Id != x.Id), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<MeasurementUnit>(x => x.NameEn == name && req.Id != x.Id), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}

public class UpdateMeasurementUnitRequestHandler(
    IRepositoryWithEvents<MeasurementUnit> repository,
    IFileStorageManager fileStorageManager,
    IStringLocalizer<UpdateMeasurementUnitRequestHandler> localizer) : IRequestHandler<UpdateMeasurementUnitRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<MeasurementUnit> _repository = repository;
    private readonly IStringLocalizer<UpdateMeasurementUnitRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<Guid>> Handle(UpdateMeasurementUnitRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["MeasurementUnit not found"]);

        part.NameAr = request.NameAr;
        part.NameEn = request.NameEn;
        part.Code = request.Code;
        part.IconUrl = request.Icon == null ? part.IconUrl : await _fileStorageManager.UploadAsync<MeasurementUnit>(request.Icon, FileType.Image, ModuleConstant.ModuleName, cancellationToken);

        await _repository.UpdateAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<Guid>.SuccessAsync(part.Id);
    }
}

