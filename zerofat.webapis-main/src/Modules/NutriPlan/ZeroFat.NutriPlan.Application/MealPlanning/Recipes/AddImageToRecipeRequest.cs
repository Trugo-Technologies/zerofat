using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Recipes;
public class AddImageToRecipeRequest : ICommand<Result<Guid>>
{
    public Guid Id { get; set; }
    public IFormFile? Image { get; set; }
}

public class AddImageToRecipeRequestHandler(
    IRepositoryWithEvents<Recipe> repository,
    IFileStorageManager fileStorageManager,
    IStringLocalizer<AddImageToRecipeRequestHandler> localizer) : IRequestHandler<AddImageToRecipeRequest, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(AddImageToRecipeRequest request, CancellationToken cancellationToken)
    {
        var recipe = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = recipe ?? throw new NotFoundException(localizer["Recipe not found"]);

        recipe.ImageUrl = request.Image == null ? recipe.ImageUrl : await fileStorageManager.UploadAsync<Recipe>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);

        await repository.UpdateAsync(recipe, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<Guid>.SuccessAsync(recipe.Id);
    }
}

