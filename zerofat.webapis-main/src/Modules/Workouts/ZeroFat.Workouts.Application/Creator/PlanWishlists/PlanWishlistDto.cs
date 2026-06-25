using ZeroFat.Application.Common.Interfaces;
using ZeroFat.GymUp.Application.Creator.Plans;

namespace ZeroFat.GymUp.Application.Creator.PlanWishlists;

public class PlanWishlistSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PlanId { get; set; }
    public DefaultIdType? UserId { get; set; }
    public PlanSimplifyDto? Plan { get; set; }
}
