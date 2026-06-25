using ZeroFat.Users.Infrastructure.Errors;
using ZeroFat.Users.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZeroFat.Users.Infrastructure.Identity.Jwt.Providers;
using ZeroFat.Shared.Authorization;

namespace ZeroFat.Users.Infrastructure.Identity;

internal static class IdentityModule
{
    internal static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        return services.AddIdentity<UsersContext>();
    }

    private static IServiceCollection AddIdentity<T>(this IServiceCollection services) where T : DbContext
    {
        var lockoutOptions = new LockoutOptions
        {
            AllowedForNewUsers = true,
            DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15),
            MaxFailedAccessAttempts = 3
        };

        services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
                config.Password.RequiredLength = 8;
                config.Password.RequireLowercase = true;
                config.Password.RequireUppercase = true;
                config.Password.RequireDigit = true;
                config.Password.RequireNonAlphanumeric = false;
                config.Lockout = lockoutOptions;
                config.Stores.MaxLengthForKeys = 85;
            })
            //.AddUserManager<ApplicationUserManager>()
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddEntityFrameworkStores<T>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<FourDigitTokenProvider>(ZeroFatProviders.FourDigitEmailProvider)
            .AddTokenProvider<FourDigitTokenProvider>(ZeroFatProviders.FourDigitPhoneProvider);


        return services;
    }
}
