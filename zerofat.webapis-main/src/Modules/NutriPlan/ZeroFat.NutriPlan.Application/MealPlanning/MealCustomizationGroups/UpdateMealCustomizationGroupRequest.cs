using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationGroups;
public class UpdateMealCustomizationGroupRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public IFormFile? Image { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; } // Whether user must select from this group
    public int MinSelection { get; set; } // Minimum options to select
    public int? MaxSelection { get; set; } // Maximum options allowed (0 for unlimited)

    public double CaloriesAdjustment { get; set; }
    public double FatAdjustment { get; set; }
    public double CarbsAdjustment { get; set; }
    public double ProteinAdjustment { get; set; }
}

public class UpdateMealCustomizationGroupRequestValidator : CustomValidator<UpdateMealCustomizationGroupRequest>
{
    public UpdateMealCustomizationGroupRequestValidator(IReadRepository<MealCustomizationGroup> repository, IStringLocalizer<UpdateMealCustomizationGroupRequestValidator> localaizer)
    {


    }
}

public class UpdateMealCustomizationGroupRequestHandler(IRepositoryWithEvents<MealCustomizationGroup> repository, IFileStorageManager fileStorageManager, IStringLocalizer<UpdateMealCustomizationGroupRequestHandler> localizer) : IRequestHandler<UpdateMealCustomizationGroupRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<MealCustomizationGroup> _repository = repository;
    private readonly IStringLocalizer<UpdateMealCustomizationGroupRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(UpdateMealCustomizationGroupRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["MealCustomizationGroup not found"]);

        part.NameAr = request.NameAr;
        part.NameEn = request.NameEn;
        part.DisplayOrder = request.DisplayOrder;
        part.MaxSelection = request.MaxSelection;
        part.MinSelection = request.MinSelection;
        part.IsRequired = request.IsRequired;
        part.FatAdjustment = request.FatAdjustment;
        part.ProteinAdjustment = request.ProteinAdjustment;
        part.CarbsAdjustment = request.CarbsAdjustment;
        part.CaloriesAdjustment = request.CaloriesAdjustment;

        part.ImageUrl = request.Image == null ? part.ImageUrl : await _fileStorageManager.UploadAsync<MealCustomizationGroup>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);

        await _repository.UpdateAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }
}

