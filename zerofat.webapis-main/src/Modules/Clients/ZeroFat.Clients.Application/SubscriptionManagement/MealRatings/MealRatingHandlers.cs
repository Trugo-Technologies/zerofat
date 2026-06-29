using Ardalis.Specification;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.MealRatings;

public class SubmitMealRatingRequest : ICommand<Result<MealRatingSummaryDto>>
{
    public DefaultIdType DailyMealSelectionId { get; set; }
    public MealRatingValue Rating { get; set; }
    public List<MealRatingImprovementTag> ImprovementTags { get; set; } = [];
    public string? Comment { get; set; }
}

public class SubmitMealRatingRequestValidator : CustomValidator<SubmitMealRatingRequest>
{
    public SubmitMealRatingRequestValidator(IStringLocalizer<SubmitMealRatingRequestValidator> localizer)
    {
        RuleFor(x => x.DailyMealSelectionId).NotEmpty();
        RuleFor(x => x.Rating)
            .IsInEnum()
            .WithMessage(localizer["A valid rating is required."]);
        RuleFor(x => x.ImprovementTags)
            .NotEmpty()
            .When(x => x.Rating is MealRatingValue.NotGood or MealRatingValue.Okay)
            .WithMessage(localizer["Please select at least one improvement area."]);
        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .WithMessage(localizer["Comment cannot exceed 2000 characters."]);
    }
}

public class DailyMealSelectionForRatingSpec : Specification<DailyMealSelection>
{
    public DailyMealSelectionForRatingSpec(DefaultIdType dailyMealSelectionId, DefaultIdType clientId)
    {
        Query
            .Where(x => x.Id == dailyMealSelectionId && x.ClientId == clientId)
            .Include(x => x.DailySelection)
            .Include(x => x.Meal);
    }
}

public class SubmitMealRatingRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<DailyMealSelection> dailyMealSelectionRepo,
    IReadRepository<MealRating> mealRatingReadRepo,
    IRepositoryWithEvents<MealRating> mealRatingRepo,
    IStringLocalizer<SubmitMealRatingRequestHandler> localizer)
    : ICommandHandler<SubmitMealRatingRequest, Result<MealRatingSummaryDto>>
{
    public async Task<Result<MealRatingSummaryDto>> Handle(
        SubmitMealRatingRequest request,
        CancellationToken cancellationToken)
    {
        if (!string.Equals(currentUser.GetRoleType(), nameof(UserType.Client), StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenException(localizer["Only clients can submit meal ratings."]);
        }

        var clientId = currentUser.GetUserId();
        var mealSelection = await dailyMealSelectionRepo.FirstOrDefaultAsync(
            new DailyMealSelectionForRatingSpec(request.DailyMealSelectionId, clientId),
            cancellationToken)
            ?? throw new NotFoundException(localizer["Meal not found."]);

        if (mealSelection.DailySelection?.DailySelectionStatus != DailySelectionStatus.Delivered)
        {
            throw new ForbiddenException(localizer["You can only rate delivered meals."]);
        }

        var mealName = mealSelection.Meal?.NameEn
            ?? mealSelection.CustomeMealName
            ?? "Meal";

        var tags = request.Rating == MealRatingValue.Awesome
            ? []
            : request.ImprovementTags.Distinct().ToList();

        var existing = await mealRatingReadRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<MealRating>(x => x.DailyMealSelectionId == request.DailyMealSelectionId),
            cancellationToken);

        if (existing != null)
        {
            existing.Rating = request.Rating;
            existing.ImprovementTags = tags;
            existing.Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();
            existing.MealName = mealName;
            existing.MealId = mealSelection.MealId;
            existing.MealDate = mealSelection.Date;

            await mealRatingRepo.UpdateAsync(existing, cancellationToken);
            return await Result<MealRatingSummaryDto>.SuccessAsync(MealRatingHelper.ToSummaryDto(existing));
        }

        var rating = new MealRating
        {
            ClientId = clientId,
            DailyMealSelectionId = request.DailyMealSelectionId,
            MealId = mealSelection.MealId,
            MealName = mealName,
            MealDate = mealSelection.Date,
            Rating = request.Rating,
            ImprovementTags = tags,
            Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim()
        };

        await mealRatingRepo.AddAsync(rating, cancellationToken);
        return await Result<MealRatingSummaryDto>.SuccessAsync(MealRatingHelper.ToSummaryDto(rating));
    }
}

public class GetMealRatingRequest(DefaultIdType dailyMealSelectionId) : IQuery<Result<MealRatingSummaryDto>>
{
    public DefaultIdType DailyMealSelectionId { get; set; } = dailyMealSelectionId;
}

public class GetMealRatingRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<MealRating> repository,
    IReadRepository<DailyMealSelection> dailyMealSelectionRepo,
    IStringLocalizer<GetMealRatingRequestHandler> localizer)
    : IQueryHandler<GetMealRatingRequest, Result<MealRatingSummaryDto>>
{
    public async Task<Result<MealRatingSummaryDto>> Handle(
        GetMealRatingRequest request,
        CancellationToken cancellationToken)
    {
        var isClient = string.Equals(currentUser.GetRoleType(), nameof(UserType.Client), StringComparison.OrdinalIgnoreCase);
        if (isClient)
        {
            var ownsMeal = await dailyMealSelectionRepo.AnyAsync(
                new ExpressionSpecification<DailyMealSelection>(x =>
                    x.Id == request.DailyMealSelectionId && x.ClientId == currentUser.GetUserId()),
                cancellationToken);

            if (!ownsMeal)
            {
                throw new NotFoundException(localizer["Meal not found."]);
            }
        }
        else
        {
            SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        }

        var rating = await repository.FirstOrDefaultAsync(
            new ExpressionSpecification<MealRating>(x => x.DailyMealSelectionId == request.DailyMealSelectionId),
            cancellationToken);

        if (rating == null)
        {
            return await Result<MealRatingSummaryDto>.SuccessAsync(new MealRatingSummaryDto());
        }

        return await Result<MealRatingSummaryDto>.SuccessAsync(MealRatingHelper.ToSummaryDto(rating));
    }
}

public class SearchMealRatingsRequest : PaginationFilter, IQuery<PaginationResponse<MealRatingDto>>
{
    public string? Search { get; set; }
    public MealRatingValue? Rating { get; set; }
    public DefaultIdType? ClientId { get; set; }
    public DefaultIdType? MealId { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }

    internal MealRatingFilterDto ToFilters() => new()
    {
        Search = Search,
        Rating = Rating,
        ClientId = ClientId,
        MealId = MealId,
        DateFrom = DateFrom,
        DateTo = DateTo
    };
}

public class MealRatingsBySearchSpec : Specification<MealRating>
{
    public MealRatingsBySearchSpec(SearchMealRatingsRequest request)
    {
        Query
            .Include(x => x.Client)
            .AsSplitQuery();

        MealRatingHelper.ApplyFilters(Query, request.ToFilters());
        Query.OrderByDescending(x => x.CreatedOn, !request.HasOrderBy())
            .PaginateBy(request);
    }
}

public class SearchMealRatingsRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<MealRating> repository,
    IStringLocalizer<SearchMealRatingsRequestHandler> localizer)
    : IQueryHandler<SearchMealRatingsRequest, PaginationResponse<MealRatingDto>>
{
    public async Task<PaginationResponse<MealRatingDto>> Handle(
        SearchMealRatingsRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);

        if (request.PageSize <= 0 || request.PageSize == int.MaxValue)
        {
            request.PageSize = 10;
        }

        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var spec = new MealRatingsBySearchSpec(request);
        var count = await repository.CountAsync(spec, cancellationToken);
        var items = await repository.ListAsync(spec, cancellationToken);

        return new PaginationResponse<MealRatingDto>(
            items.Select(MealRatingHelper.ToAdminDto).ToList(),
            count,
            pageNumber,
            request.PageSize);
    }
}

public class ReplyMealRatingRequest : ICommand<Result<MealRatingDto>>
{
    public DefaultIdType Id { get; set; }
    public string Reply { get; set; } = string.Empty;
}

public class ReplyMealRatingRequestValidator : CustomValidator<ReplyMealRatingRequest>
{
    public ReplyMealRatingRequestValidator(IStringLocalizer<ReplyMealRatingRequestValidator> localizer)
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reply)
            .NotEmpty()
            .WithMessage(localizer["Reply is required."])
            .MaximumLength(2000)
            .WithMessage(localizer["Reply cannot exceed 2000 characters."]);
    }
}

public class MealRatingByIdSpec : Specification<MealRating>
{
    public MealRatingByIdSpec(DefaultIdType id)
    {
        Query.Where(x => x.Id == id).Include(x => x.Client);
    }
}

public class ReplyMealRatingRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<MealRating> readRepository,
    IRepositoryWithEvents<MealRating> repository,
    IStringLocalizer<ReplyMealRatingRequestHandler> localizer)
    : ICommandHandler<ReplyMealRatingRequest, Result<MealRatingDto>>
{
    public async Task<Result<MealRatingDto>> Handle(
        ReplyMealRatingRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);

        var rating = await readRepository.FirstOrDefaultAsync(
            new MealRatingByIdSpec(request.Id),
            cancellationToken)
            ?? throw new NotFoundException(localizer["Meal rating not found."]);

        rating.AdminReply = request.Reply.Trim();
        rating.AdminRepliedOn = DateTime.UtcNow;
        rating.AdminRepliedByName = currentUser.Name;

        await repository.UpdateAsync(rating, cancellationToken);
        return await Result<MealRatingDto>.SuccessAsync(MealRatingHelper.ToAdminDto(rating));
    }
}
