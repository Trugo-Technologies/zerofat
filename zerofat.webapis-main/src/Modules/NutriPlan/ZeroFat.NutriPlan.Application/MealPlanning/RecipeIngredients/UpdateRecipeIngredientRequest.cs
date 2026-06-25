using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MealPlanning.RecipeIngredients;

public class UpdateRecipeIngredientRequest : ICommand<Result<Guid>>
{
    public Guid Id { get; set; }
    public string MeasurementUnitCode { get; set; }
    public bool HideOnCustomerPorter { get; set; }
    public double Amount { get; set; }
    public bool IsOptional { get; set; }
}

public class UpdateRecipeIngredientRequestValidator : CustomValidator<UpdateRecipeIngredientRequest>
{
    public UpdateRecipeIngredientRequestValidator(
        IReadRepository<RecipeIngredient> repository, 
        IReadRepository<MeasurementUnit> MeasurementUnitRepo, 
        IReadRepository<Ingredient> IngredientRepo, 
        IStringLocalizer<UpdateRecipeIngredientRequestValidator> localaizer)
    {

        RuleFor(u => u.MeasurementUnitCode)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await MeasurementUnitRepo.AnyAsync(new ExpressionSpecification<MeasurementUnit>(x => x.Code == id), _))
                .WithMessage(localaizer["MeasurementUnit not found"]);

        RuleFor(u => true)
          .Cascade(CascadeMode.Stop)
          .MustAsync(async (req, _, c) => !await repository.AnyAsync(new ExpressionSpecification<RecipeIngredient>(x => x.MeasurementUnitCode == req.MeasurementUnitCode && x.Id != req.Id), c))
               .WithMessage(localaizer["MeasurementUnit is already attached with this Ingredient"]);
    }
}


public class UpdateRecipeIngredientRequestHandler(
    IRepositoryWithEvents<RecipeIngredient> repository,
    IRepositoryWithEvents<Ingredient> ingredientRepo,
    IRepositoryWithEvents<IngredientMeasurementUnit> ingredientMeasurementUnitRepo
    ) : IRequestHandler<UpdateRecipeIngredientRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<Ingredient> _ingredientRepo = ingredientRepo;
    private readonly IRepositoryWithEvents<RecipeIngredient> _repository = repository;
    private readonly IRepositoryWithEvents<IngredientMeasurementUnit> _ingredientMeasurementUnitRepo = ingredientMeasurementUnitRepo;

    public async Task<Result<Guid>> Handle(UpdateRecipeIngredientRequest request, CancellationToken cancellationToken)
    {
        var recipeIngredient = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = recipeIngredient ?? throw new NotFoundException("RecipeIngredient not found");

        var ingredient = await _ingredientRepo.GetByIdAsync(recipeIngredient.IngredientId, cancellationToken);
        var ingredientUnitMeasurement = await _ingredientMeasurementUnitRepo.FirstOrDefaultAsync(new ExpressionSpecification<IngredientMeasurementUnit>(x => x.IngredientId == recipeIngredient.IngredientId && x.Code == request.MeasurementUnitCode), cancellationToken);

        recipeIngredient.MeasurementUnitCode = request.MeasurementUnitCode;
        recipeIngredient.Amount = request.Amount;
        recipeIngredient.IsOptional = request.IsOptional;

        recipeIngredient.WeightInGrams = request.Amount;

        if (ingredientUnitMeasurement != null)
            recipeIngredient.WeightInGrams *= ingredientUnitMeasurement.EquivalentInUnit;
        if (ingredient.BasicUnit == BasicUnitType.Liquid)
            recipeIngredient.WeightInGrams *= ingredient.Density;

        await _repository.UpdateAsync(recipeIngredient, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<Guid>.SuccessAsync(recipeIngredient.Id);
    }

}
