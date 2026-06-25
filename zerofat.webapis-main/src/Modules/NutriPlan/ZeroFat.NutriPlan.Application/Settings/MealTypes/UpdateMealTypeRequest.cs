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

namespace ZeroFat.NutriPlan.Application.Settings.MealTypes;
public class UpdateMealTypeRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? Icon { get; set; }
    public IFormFile? Image { get; set; }
}

public class UpdateMealTypeRequestValidator : CustomValidator<UpdateMealTypeRequest>
{
    public UpdateMealTypeRequestValidator(IReadRepository<MealType> repository, IStringLocalizer<UpdateMealTypeRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<MealType>(x => x.NameAr == name && req.Id != x.Id), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<MealType>(x => x.NameEn == name && req.Id != x.Id), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}

public class UpdateMealTypeRequestHandler(
    IRepositoryWithEvents<MealType> repository,
    IFileStorageManager fileStorageManager,
    IStringLocalizer<UpdateMealTypeRequestHandler> localizer) : IRequestHandler<UpdateMealTypeRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<MealType> _repository = repository;
    private readonly IStringLocalizer<UpdateMealTypeRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(UpdateMealTypeRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["MealType not found"]);

        part.NameAr = request.NameAr;
        part.NameEn = request.NameEn;
        part.IconUrl = request.Icon == null ? part.IconUrl : await _fileStorageManager.UploadAsync<MealType>(request.Icon, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        part.ImageUrl = request.Image == null ? part.ImageUrl : await _fileStorageManager.UploadAsync<MealType>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);

        if (part.IsDefault)
            throw new BadRequestException(_localizer["Default MeasurementUnit can't be deleted"]);

        await _repository.UpdateAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }
}

