using ZeroFat.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace ZeroFat.Infrastructure.Api.Auth;

internal static class Extensions
{
    internal static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app) =>
      app.UseMiddleware<CurrentUserMiddleware>()
         .UseMiddleware<AccountStatusMiddleware>();

    internal static IServiceCollection AddCurrentUser(this IServiceCollection services) =>
        services
            .AddScoped<CurrentUserMiddleware>()
            .AddScoped<AccountStatusMiddleware>()
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());

    internal static IApplicationBuilder UseFileStorage(this IApplicationBuilder app) =>
       app.UseStaticFiles(new StaticFileOptions()
       {
           FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Files")),
           RequestPath = new PathString("/Files")
       });

}
