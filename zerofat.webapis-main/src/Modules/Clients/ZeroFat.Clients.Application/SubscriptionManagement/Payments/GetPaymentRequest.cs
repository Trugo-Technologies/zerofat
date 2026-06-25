using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Payments;

public class GetPaymentRequest(DefaultIdType id) : IQuery<Result<PaymentDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetPaymentRequestHandler(IRepositoryWithEvents<Payment> repository, IReadRepository<MealPlan> mealPlanRepo, IStringLocalizer<GetPaymentRequestHandler> localizer) : IRequestHandler<GetPaymentRequest, Result<PaymentDetailsDto>>
{
    private readonly IRepositoryWithEvents<Payment> _repository = repository;
    private readonly IReadRepository<MealPlan> _mealPlanRepo = mealPlanRepo;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<PaymentDetailsDto>> Handle(GetPaymentRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new PaymentByIdSpec<PaymentDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Payment not found", request.Id]);

        // entity.MealPlan = await _mealPlanRepo.FirstOrDefaultAsync(new MealPlanByIdSpec<MealPlanDto>(entity.MealPlanId), cancellationToken);

        return await Result<PaymentDetailsDto>.SuccessAsync(entity);
    }

}
