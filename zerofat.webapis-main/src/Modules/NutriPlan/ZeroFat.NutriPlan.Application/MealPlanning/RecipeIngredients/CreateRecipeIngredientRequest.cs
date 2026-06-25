using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MealPlanning.RecipeIngredients;

public class CreateRecipeIngredientRequest : ICommand<Result<Guid>>
{
    public Guid IngredientId { get; set; }
    public string MeasurementUnitCode { get; set; }
    public Guid RecipeId { get; set; }
    public bool IsOptional { get; set; }
    public bool HideOnCustomerPorter { get; set; }
    public double Amount { get; set; }
}

public class CreateRecipeIngredientRequestValidator : CustomValidator<CreateRecipeIngredientRequest>
{
    public CreateRecipeIngredientRequestValidator(
        IReadRepository<RecipeIngredient> repository,
        IReadRepository<MeasurementUnit> MeasurementUnitRepo,
        IReadRepository<Ingredient> IngredientRepo,
        IReadRepository<Recipe> RecipeRepo,
        IStringLocalizer<CreateRecipeIngredientRequestValidator> localaizer)
    {

        RuleFor(u => u.IngredientId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await IngredientRepo.AnyAsync(new ExpressionSpecification<Ingredient>(x => x.Id == id), _))
                .WithMessage(localaizer["Ingredient not found"]);

        RuleFor(u => u.MeasurementUnitCode)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (code, _) => await MeasurementUnitRepo.AnyAsync(new ExpressionSpecification<MeasurementUnit>(x => x.Code == code), _))
                .WithMessage(localaizer["MeasurementUnit not found"]);

        RuleFor(u => u.RecipeId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await RecipeRepo.AnyAsync(new ExpressionSpecification<Recipe>(x => x.Id == id), _))
                .WithMessage(localaizer["Recipe not found"]);

        RuleFor(u => true)
          .Cascade(CascadeMode.Stop)
          .MustAsync(async (req, _, c) => !await repository.AnyAsync(new ExpressionSpecification<RecipeIngredient>(x => x.MeasurementUnitCode == req.MeasurementUnitCode && x.IngredientId == req.IngredientId && x.RecipeId == req.RecipeId), c))
               .WithMessage(localaizer["MeasurementUnit is already attached with this Ingredient"]);
    }
}


public class CreateRecipeIngredientRequestHandler(
    IRepositoryWithEvents<RecipeIngredient> repository,
    IRepositoryWithEvents<Ingredient> ingredientRepo,
    IRepositoryWithEvents<IngredientMeasurementUnit> ingredientMeasurementUnitRepo
    ) : IRequestHandler<CreateRecipeIngredientRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<RecipeIngredient> _repository = repository;
    private readonly IRepositoryWithEvents<Ingredient> _ingredientRepo = ingredientRepo;
    private readonly IRepositoryWithEvents<IngredientMeasurementUnit> _ingredientMeasurementUnitRepo = ingredientMeasurementUnitRepo;

    public async Task<Result<Guid>> Handle(CreateRecipeIngredientRequest request, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepo.GetByIdAsync(request.IngredientId, cancellationToken);
        var ingredientUnitMeasurement = await _ingredientMeasurementUnitRepo.FirstOrDefaultAsync(new ExpressionSpecification<IngredientMeasurementUnit>(x => x.IngredientId == request.IngredientId && x.Code == request.MeasurementUnitCode), cancellationToken);

        var recipeIngredient = new RecipeIngredient
        {
            MeasurementUnitCode = request.MeasurementUnitCode,
            RecipeId = request.RecipeId,
            IngredientId = request.IngredientId,
            IsOptional = request.IsOptional,
            HideOnCustomerPorter = request.HideOnCustomerPorter,
            Amount = request.Amount,
            BasicUnit = ingredient.BasicUnit == BasicUnitType.Liquid ? BasicUnit.ml : BasicUnit.g,
        };

        recipeIngredient.WeightInGrams = request.Amount;
        if (ingredientUnitMeasurement != null)
            recipeIngredient.WeightInGrams *= ingredientUnitMeasurement.EquivalentInUnit;

        if (ingredient.BasicUnit == BasicUnitType.Liquid)
            recipeIngredient.WeightInGrams *= ingredient.Density;

        await _repository.AddAsync(recipeIngredient, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<Guid>.SuccessAsync(recipeIngredient.Id);
    }

}
