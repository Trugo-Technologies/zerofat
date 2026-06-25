using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.NutrientsAttributes;

public class DeleteNutrientsAttributeRequest : ICommand<Result<Guid>>
{
    public Guid Id { get; set; }
    public DeleteNutrientsAttributeRequest(Guid id) => Id = id;
}


public class DeleteNutrientsAttributeRequestHandler(IRepository<NutrientsAttribute> repository, IStringLocalizer<DeleteNutrientsAttributeRequestHandler> localizer) : IRequestHandler<DeleteNutrientsAttributeRequest, Result<Guid>>
{
    private readonly IRepository<NutrientsAttribute> _repository = repository;
    private readonly IStringLocalizer<DeleteNutrientsAttributeRequestHandler> _localizer = localizer;

    public async Task<Result<Guid>> Handle(DeleteNutrientsAttributeRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["NutrientsAttribute not found"]);

        await _repository.DeleteAsync(part, cancellationToken);

        return await Result<Guid>.SuccessAsync(part.Id);
    }

}
