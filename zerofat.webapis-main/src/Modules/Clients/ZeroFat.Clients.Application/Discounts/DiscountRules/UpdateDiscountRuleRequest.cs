using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Domain.Discounts;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.Discounts.DiscountRules;

public class UpdateDiscountRuleRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public string Code { get; set; } = null!;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;

    public long? PercentOff { get; set; }
    public long? AmountOff { get; set; }

    public DiscountDuration Duration { get; set; } = DiscountDuration.Once;
    public int DurationInMonths { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }

    public int MaxRedemptions { get; set; }
    public int MaxRedemptionsPerClient { get; set; } = 1;

    public bool FirstTimeClientsOnly { get; set; }
}

public class UpdateDiscountRuleRequestValidator : CustomValidator<UpdateDiscountRuleRequest>
{
    public UpdateDiscountRuleRequestValidator(
        IReadRepository<DiscountRule> discountRepo,
        IStringLocalizer<UpdateDiscountRuleRequestValidator> localizer)
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage(localizer["Discount code is required."])
            .MustAsync(async (req, code, ct) =>
                !await discountRepo.AnyAsync(new ExpressionSpecification<DiscountRule>(
                    x => x.Code == code && x.Id != req.Id), ct))
            .WithMessage(localizer["Discount code already exists."]);

        RuleFor(x => x.PercentOff)
            .InclusiveBetween(0, 100)
            .When(x => x.PercentOff.HasValue);

        RuleFor(x => x.AmountOff)
            .GreaterThan(0)
            .When(x => x.AmountOff.HasValue);

        RuleFor(x => x)
            .Must(x => x.PercentOff.HasValue || x.AmountOff.HasValue)
            .WithMessage(localizer["Either PercentOff or AmountOff must be provided."]);

        RuleFor(x => x.DurationInMonths)
            .GreaterThan(0)
            .When(x => x.Duration == DiscountDuration.Repeating);

        RuleFor(x => x.ExpirationDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .When(x => x.ExpirationDate.HasValue);

        RuleFor(x => x.MaxRedemptions)
            .GreaterThan(0);

        RuleFor(x => x.MaxRedemptionsPerClient)
            .GreaterThan(0);
    }
}

public class UpdateDiscountRuleRequestHandler(IRepositoryWithEvents<DiscountRule> repository)
    : IRequestHandler<UpdateDiscountRuleRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<DiscountRule> _repository = repository;

    public async Task<Result<DefaultIdType>> Handle(UpdateDiscountRuleRequest request, CancellationToken cancellationToken)
    {
        var discount = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (discount is null)
            return await Result<DefaultIdType>.FailAsync("Discount rule not found.");

        discount.Code = request.Code;
        discount.DescriptionEn = request.DescriptionEn;
        discount.DescriptionAr = request.DescriptionAr;
        discount.PercentOff = request.PercentOff;
        discount.AmountOff = request.AmountOff;
        discount.Duration = request.Duration;
        discount.DurationInMonths = request.Duration == DiscountDuration.Repeating ? request.DurationInMonths : 0;
        discount.StartDate = request.StartDate;
        discount.ExpirationDate = request.ExpirationDate;
        discount.MaxRedemptions = request.MaxRedemptions;
        discount.MaxRedemptionsPerClient = request.MaxRedemptionsPerClient;
        discount.FirstTimeClientsOnly = request.FirstTimeClientsOnly;

        await _repository.UpdateAsync(discount, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(discount.Id);
    }
}

