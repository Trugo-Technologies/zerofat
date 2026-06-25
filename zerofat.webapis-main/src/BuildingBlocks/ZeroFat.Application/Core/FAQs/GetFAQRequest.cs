using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.FAQs;

public class GetFaqRequest(DefaultIdType id) : IQuery<Result<FaqDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetFaqRequestHandler(IReadRepository<Faq> repository, IStringLocalizer<GetFaqRequestHandler> localizer) : IRequestHandler<GetFaqRequest, Result<FaqDetailsDto>>
{
    private readonly IReadRepository<Faq> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<FaqDetailsDto>> Handle(GetFaqRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new FaqByIdSpec<FaqDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["FAQ not found", request.Id]);

        return await Result<FaqDetailsDto>.SuccessAsync(entity);
    }

}
