using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ZeroFat.Infrastructure.Api.Swagger;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class DeviceHeaderAttribute : Attribute
{
    public DeviceHeaderAttribute(string name, Type parameterType, string? description = null, bool required = false, object? defaultValue = null)
    {
        Name = name;
        Description = description;
        Required = required;
        ParameterType = parameterType;
        DefaultValue = defaultValue;
    }

    public string Name { get; }
    public string? Description { get; }
    public bool Required { get; }
    public Type ParameterType { get; }
    public object? DefaultValue { get; }
}

public sealed class DeviceHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attrs = context.MethodInfo.GetCustomAttributes(true).OfType<DeviceHeaderAttribute>();

        foreach (var attr in attrs)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = attr.Name,
                In = ParameterLocation.Header,
                Description = attr.Description,
                Required = attr.Required,
                Schema = GetSchema(attr.ParameterType, attr.DefaultValue)
            });
        }
        
    }

    private OpenApiSchema GetSchema(Type parameterType, object? defaultValue)
    {
        var schema = new OpenApiSchema { Type = GetParameterType(parameterType) };

        if (parameterType.IsEnum)
        {
            schema.Enum = GetEnumValues(parameterType);
            schema.Default = new OpenApiString(defaultValue?.ToString());
        }
        else
        {
            schema.Default = new OpenApiString(defaultValue?.ToString());
        }

        return schema;
    }

    private string GetParameterType(Type parameterType)
    {
        if (parameterType == typeof(string))
        {
            return "string";
        }
        else if (parameterType == typeof(int))
        {
            return "integer";
        }
        else
        {
            return "string";
        }
    }

    private IList<IOpenApiAny> GetEnumValues(Type parameterType)
    {
        var enumValues = Enum.GetValues(parameterType);
        var enumList = new List<IOpenApiAny>();

        foreach (var value in enumValues)
        {
            var enumString = Enum.GetName(parameterType, value);
            enumList.Add(new OpenApiString(enumString));
        }

        return enumList;
    }
}
