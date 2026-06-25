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

namespace ZeroFat.NutriPlan.Application.MealPlanning.Extras;
public class UpdateExtraRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
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
}

public class UpdateExtraRequestValidator : CustomValidator<UpdateExtraRequest>
{
    public UpdateExtraRequestValidator(IReadRepository<Extra> repository, IStringLocalizer<UpdateExtraRequestValidator> localaizer)
    {


    }
}

public class UpdateExtraRequestHandler(IRepositoryWithEvents<Extra> repository, IFileStorageManager fileStorageManager, IStringLocalizer<UpdateExtraRequestHandler> localizer) : IRequestHandler<UpdateExtraRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Extra> _repository = repository;
    private readonly IStringLocalizer<UpdateExtraRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(UpdateExtraRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["Extra not found"]);

        part.NameAr = request.NameAr;
        part.NameEn = request.NameEn;
        part.Price = request.Price;
        part.Amount = request.Amount;
        part.WeightInGrams = request.WeightInGrams;
        part.OrginalIngredientId = request.OrginalIngredientId;
        part.IngredientId = request.IngredientId;
        part.ImageUrl = request.Image == null ? part.ImageUrl : await _fileStorageManager.UploadAsync<Extra>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);

        await _repository.UpdateAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }
}

