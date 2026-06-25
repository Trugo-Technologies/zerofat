using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;

namespace ZeroFat.GymUp.Application.Creator.PlanWishlists;

public class CreateRemovePlanWishlistRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType PlanId { get; set; }
    public CreateRemovePlanWishlistRequest(DefaultIdType planId) => PlanId = planId;
}

public class CreatePlanWishlistRequestValidator : CustomValidator<CreateRemovePlanWishlistRequest>
{
    public CreatePlanWishlistRequestValidator(IReadRepository<Plan> repository, IStringLocalizer<CreatePlanWishlistRequestValidator> localaizer)
    {

        RuleFor(u => u.PlanId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await repository.AnyAsync(new ExpressionSpecification<Plan>(x => x.Id == id), _))
                .WithMessage(localaizer["Plan not found"]);
    }
}


public class CreatePlanWishlistRequestHandler(IRepositoryWithEvents<PlanWishlist> repository, ICurrentUser currentUser) : IRequestHandler<CreateRemovePlanWishlistRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<PlanWishlist> _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<DefaultIdType>> Handle(CreateRemovePlanWishlistRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var wishlist = await _repository.FirstOrDefaultAsync(new ExpressionSpecification<PlanWishlist>(x => x.PlanId == request.PlanId && x.UserId == userId), cancellationToken);
        if (wishlist is not null)
        {
            await _repository.DeleteAsync(wishlist, true, false, cancellationToken);
        }
        else
        {
            wishlist = new PlanWishlist
            {
                PlanId = request.PlanId,
                UserId = userId
            };

            await _repository.AddAsync(wishlist, withSaveChanges: true, cancellationToken: cancellationToken);
        }

        return await Result<DefaultIdType>.SuccessAsync(wishlist.Id);
    }

}
