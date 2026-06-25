using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Domain.Discounts;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.Discounts.DiscountRules;

public class CreateDiscountRuleRequest : ICommand<Result<DefaultIdType>>
{
    public string Code { get; set; } = null!;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;

    public long? PercentOff { get; set; }
    public long? AmountOff { get; set; }

    public DiscountDuration Duration { get; set; } = DiscountDuration.Once;
    public int DurationInMonths { get; set; } // Only used if Repeating

    public DateOnly? StartDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }

    public int MaxRedemptions { get; set; }
    public int MaxRedemptionsPerClient { get; set; } = 1;

    // 🆕 Additional properties
    public bool FirstTimeClientsOnly { get; set; }      // Limit to new clients
}

public class CreateDiscountRuleRequestValidator : CustomValidator<CreateDiscountRuleRequest>
{
    public CreateDiscountRuleRequestValidator(
        IReadRepository<DiscountRule> discountRepo,
        IStringLocalizer<CreateDiscountRuleRequestValidator> localizer)
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage(localizer["Discount code is required."])
            .MustAsync(async (code, ct) =>
                !await discountRepo.AnyAsync(new ExpressionSpecification<DiscountRule>(x => x.Code == code), ct))
            .WithMessage(localizer["Discount code already exists."]);

        RuleFor(x => x.PercentOff)
            .InclusiveBetween(0, 100)
            .When(x => x.PercentOff.HasValue)
            .WithMessage(localizer["Percent off must be between 0 and 100."]);

        RuleFor(x => x.AmountOff)
            .GreaterThan(0)
            .When(x => x.AmountOff.HasValue)
            .WithMessage(localizer["Amount off must be greater than 0."]);

        RuleFor(x => x)
            .Must(x => x.PercentOff.HasValue || x.AmountOff.HasValue)
            .WithMessage(localizer["Either PercentOff or AmountOff must be provided."]);

        RuleFor(x => x.DurationInMonths)
            .GreaterThan(0)
            .When(x => x.Duration == DiscountDuration.Repeating)
            .WithMessage(localizer["Duration in months must be provided for repeating discounts."]);

        RuleFor(x => x.ExpirationDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .When(x => x.ExpirationDate.HasValue)
            .WithMessage(localizer["Expiration date must be in the future."]);

        RuleFor(x => x.MaxRedemptions)
            .GreaterThan(0).WithMessage(localizer["Max redemptions must be greater than 0."]);

        RuleFor(x => x.MaxRedemptionsPerClient)
            .GreaterThan(0).WithMessage(localizer["Max redemptions per client must be greater than 0."]);
    }
}

public class CreateDiscountRuleRequestHandler(IRepositoryWithEvents<DiscountRule> repository, IStripeService stripeService)
    : IRequestHandler<CreateDiscountRuleRequest, Result<DefaultIdType>>
{
    public async Task<Result<DefaultIdType>> Handle(CreateDiscountRuleRequest request, CancellationToken cancellationToken)
    {
        var discount = new DiscountRule
        {
            Code = request.Code,
            DescriptionEn = request.DescriptionEn,
            DescriptionAr = request.DescriptionAr,
            PercentOff = request.PercentOff,
            AmountOff = request.AmountOff,
            Duration = request.Duration,
            DurationInMonths = request.Duration == DiscountDuration.Repeating ? request.DurationInMonths : 0,
            StartDate = request.StartDate,
            ExpirationDate = request.ExpirationDate,
            MaxRedemptions = request.MaxRedemptions,
            RedemptionsUsed = 0,
            MaxRedemptionsPerClient = request.MaxRedemptionsPerClient,
            FirstTimeClientsOnly = request.FirstTimeClientsOnly,
        };

        await repository.AddAsync(discount, cancellationToken);
        return await Result<DefaultIdType>.SuccessAsync(discount.Id);
    }
}
