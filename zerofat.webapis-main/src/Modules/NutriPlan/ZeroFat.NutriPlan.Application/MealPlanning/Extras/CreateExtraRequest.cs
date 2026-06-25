using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Extras;
public class CreateExtraRequest : ICommand<Result<DefaultIdType>>
{
    public string? NameEn { get; set; } // Name of the extra (e.g., "Avocado", "Cheese")
    public string? NameAr { get; set; } // Name of the extra (e.g., "Avocado", "Cheese")
    public decimal Price { get; set; } // Additional price for adding this extra
    public IFormFile? Image { get; set; }

    /// <summary>
    /// Amount of the ingredient required for the recipe.
    /// </summary>
    public double Amount { get; set; }
    /// <summary>
    /// Equivalent weight of the ingredient in grams.
    /// </summary>
    public double WeightInGrams { get; set; }

    public DefaultIdType? OrginalIngredientId { get; set; }

    // Navigation properties
    public DefaultIdType? IngredientId { get; set; }
    public DefaultIdType MealId { get; set; }
}

public class CreateExtraRequestValidator : CustomValidator<CreateExtraRequest>
{
    public CreateExtraRequestValidator(IReadRepository<Extra> repository, IStringLocalizer<CreateExtraRequestValidator> localaizer)
    {


    }
}


public class CreateExtraRequestHandler(IRepositoryWithEvents<Extra> repository, IFileStorageManager fileStorageManager) : IRequestHandler<CreateExtraRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Extra> _repository = repository;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(CreateExtraRequest request, CancellationToken cancellationToken)
    {
        var part = new Extra
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            Price = request.Price,
            Amount = request.Amount,
            IngredientId = request.IngredientId,
            OrginalIngredientId = request.OrginalIngredientId,
            WeightInGrams = request.WeightInGrams,
            MealId = request.MealId,
            ImageUrl = request.Image == null ? null : await _fileStorageManager.UploadAsync<Extra>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
            //IconUrl = 
        };

        await _repository.AddAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
