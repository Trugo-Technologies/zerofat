using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationGroups;
public class CreateMealCustomizationGroupRequest : ICommand<Result<DefaultIdType>>
{
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

public class CreateMealCustomizationGroupRequestValidator : CustomValidator<CreateMealCustomizationGroupRequest>
{
    public CreateMealCustomizationGroupRequestValidator(IReadRepository<MealCustomizationGroup> repository, IStringLocalizer<CreateMealCustomizationGroupRequestValidator> localaizer)
    {


    }
}


public class CreateMealCustomizationGroupRequestHandler(IRepositoryWithEvents<MealCustomizationGroup> repository, IFileStorageManager fileStorageManager) : IRequestHandler<CreateMealCustomizationGroupRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<MealCustomizationGroup> _repository = repository;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(CreateMealCustomizationGroupRequest request, CancellationToken cancellationToken)
    {
        var part = new MealCustomizationGroup
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            IsRequired = request.IsRequired,
            DisplayOrder = request.DisplayOrder,
            MaxSelection = request.MaxSelection,
            MinSelection = request.MinSelection,
            FatAdjustment = request.FatAdjustment,
            ProteinAdjustment = request.ProteinAdjustment,
            CarbsAdjustment = request.CarbsAdjustment,
            CaloriesAdjustment = request.CaloriesAdjustment,
            ImageUrl = request.Image == null ? null : await _fileStorageManager.UploadAsync<MealCustomizationGroup>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
            //IconUrl = 
        };

        await _repository.AddAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
