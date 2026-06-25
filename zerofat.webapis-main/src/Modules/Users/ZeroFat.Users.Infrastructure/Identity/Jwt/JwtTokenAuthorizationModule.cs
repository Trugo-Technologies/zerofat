using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ZeroFat.Users.Infrastructure.Identity.Jwt.Configuration;
using Microsoft.AspNetCore.Authorization;
using ZeroFat.Users.Application.Authentication;
using ZeroFat.Users.Application.Contracts;

namespace ZeroFat.Users.Infrastructure.Auth.Jwt;

internal static class JwtTokenAuthorizationModule
{
    internal static IServiceCollection AddJwtTokenAuthorizationModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, TokenValidAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, SameUserAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, NotSameUserAuthorizationHandler>();

        // services.AddScoped<IJwtTokenManagementService, JwtTokenManagementService>();
        /// services.AddScoped<ITokenStoreManager, TokenStoreManager>();
        /// services.AddScoped<IRefreshTokenGenerationService, RefreshTokenGenerationService>();
        /// services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        /// services.AddSingleton<IUserIdProvider, UserPublicIdProvider>();


        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
        .ValidateDataAnnotations()
        .ValidateOnStart();

        var jwtOptions = config.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

        services
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, null!);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNameKeys.TokenValid, policy => policy.Requirements.Add(new TokenValidRequirement()));
            options.AddPolicy(PolicyNameKeys.SameUser, policy => policy.Requirements.Add(new SameUserRequirement()));
            options.AddPolicy(PolicyNameKeys.NotSameUser, policy => policy.Requirements.Add(new NotSameUserRequirement()));
        });

        services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromMinutes(jwtOptions!.LinksExpirationInMinutes));

        return services;
    }
}
