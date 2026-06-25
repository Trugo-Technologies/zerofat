namespace ZeroFat.Users.Application.Common.Extensions;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

internal static class IdentityResultExtensions
{
    public static List<string> GetErrors(this IdentityResult result, IStringLocalizer localizer) =>
        result.Errors.Select(e => localizer[e.Code].ToString()).ToList();
}
