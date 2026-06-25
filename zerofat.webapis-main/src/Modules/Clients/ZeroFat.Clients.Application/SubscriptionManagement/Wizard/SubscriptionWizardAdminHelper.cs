using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

internal static class SubscriptionWizardAdminHelper
{
    public static void EnsureAdmin(ICurrentUser currentUser, IStringLocalizer localizer)
    {
        if (!string.Equals(currentUser.GetRoleType(), nameof(UserType.Admin), StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenException(localizer["Only admins can perform this action."]);
        }
    }
}
