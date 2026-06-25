using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.Menus;
public class PublishMenuRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public PublishMenuRequest(DefaultIdType id) => Id = id;
}

public class PublishMenuRequestHandler(IRepository<Menu> repository, IStringLocalizer<PublishMenuRequestHandler> localizer) : ICommandHandler<PublishMenuRequest, Result>
{
    private readonly IRepository<Menu> _repository = repository;
    private readonly IStringLocalizer<PublishMenuRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(PublishMenuRequest request, CancellationToken cancellationToken)
    {
        var menu = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = menu ?? throw new NotFoundException(_localizer["menu not found"]);

        menu.IsPublished = !menu.IsPublished;

        await _repository.UpdateAsync(menu, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
