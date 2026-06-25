using MediatR;
using Microsoft.AspNetCore.Http;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Ingredients;

public class UploadIngredientExcelCommand : ICommand<Result>
{
    public IFormFile? File { get; set; }
}

public class UploadIngredientExcelCommandValidator : CustomValidator<UploadIngredientExcelCommand>
{
    public UploadIngredientExcelCommandValidator()
    {

        RuleFor(u => u.File)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull();
    }
}

public class UploadIngredientExcelCommandHandler : IRequestHandler<UploadIngredientExcelCommand, Result>
{
    private readonly INutriPlanExcelReader _nutriPlanExcelReader;
    private readonly IRepositoryWithEvents<Ingredient> _repository;

    public UploadIngredientExcelCommandHandler(
        INutriPlanExcelReader nutriPlanExcelReader,
        IRepositoryWithEvents<Ingredient> repository)
    {
        _nutriPlanExcelReader = nutriPlanExcelReader;
        _repository = repository;
    }

    public async Task<Result> Handle(UploadIngredientExcelCommand request, CancellationToken cancellationToken)
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xlsx");

        try
        {
            // Save the uploaded file to the temporary path
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await request.File!.CopyToAsync(stream, cancellationToken);
            }

            // Process the file
            var ingredients = _nutriPlanExcelReader.ReadIngredientsFromExcel(tempFilePath);
            // Delete the temporary file
            await _repository.AddRangeAsync(ingredients, cancellationToken);
        }
        finally
        {
            // Delete the temporary file after processing
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
        return (Result)await Result.SuccessAsync();
    }
}
