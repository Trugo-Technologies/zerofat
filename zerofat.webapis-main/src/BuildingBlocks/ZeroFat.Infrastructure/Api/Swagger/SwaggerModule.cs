using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ZeroFat.Infrastructure.Api.Swagger;
internal static class SwaggerModule
{
    internal static void AddSwaggerModule(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options => 
        {
            options.OperationFilter<SwaggerDefaultValues>();
            options.OperationFilter<DeviceHeaderOperationFilter>();

            options.AddJwtAuthorization();
            options.EnableAnnotations();
        });
        services
            .AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
            })
            .EnableApiVersionBinding();
    }

    internal static void UseSwaggerModule(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // options.DefaultModelExpandDepth(-1);
            options.DefaultModelsExpandDepth(-1);
            options.DisplayRequestDuration();
            // 
            options.EnableFilter();
            options.EnableValidator();
            options.DocExpansion(DocExpansion.None);
            options.EnableDeepLinking();

            var swaggerEndpoints = app.DescribeApiVersions()
                .Select(desc => new
                {
                    Url = $"/swagger/{desc.GroupName}/swagger.json",
                    Name = desc.GroupName.ToUpperInvariant()
                });

            foreach (var endpoint in swaggerEndpoints)
            {
                options.SwaggerEndpoint(endpoint.Url, endpoint.Name);
            }
        });
    }
}
