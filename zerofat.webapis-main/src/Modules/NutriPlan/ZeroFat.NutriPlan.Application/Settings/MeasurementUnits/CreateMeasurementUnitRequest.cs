using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MeasurementUnits;
public class CreateMeasurementUnitRequest : ICommand<Result<DefaultIdType>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string Code { get; set; } = default!;
    public IFormFile? Icon { get; set; }
}

public class CreateMeasurementUnitRequestValidator : CustomValidator<CreateMeasurementUnitRequest>
{
    public CreateMeasurementUnitRequestValidator(IReadRepository<MeasurementUnit> repository, IStringLocalizer<CreateMeasurementUnitRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<MeasurementUnit>(x => x.NameAr == name), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<MeasurementUnit>(x => x.Code == name), _))
                 .WithMessage(localaizer["Code already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<MeasurementUnit>(x => x.NameEn == name), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}


public class CreateMeasurementUnitRequestHandler(IRepositoryWithEvents<MeasurementUnit> repository, IFileStorageManager fileStorageManager) : ICommandHandler<CreateMeasurementUnitRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<MeasurementUnit> _repository = repository;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(CreateMeasurementUnitRequest request, CancellationToken cancellationToken)
    {
        var part = new MeasurementUnit
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            Code = request.Code,
            IconUrl = request.Icon == null ? null : await _fileStorageManager.UploadAsync<MeasurementUnit>(request.Icon, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
        };

        await _repository.AddAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
