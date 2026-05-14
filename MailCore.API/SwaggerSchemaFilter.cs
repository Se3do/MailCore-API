using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MailCore.API;

public class SwaggerSchemaFilter : ISchemaFilter
{
    private static readonly Dictionary<string, Func<IOpenApiAny>> Examples = new(StringComparer.OrdinalIgnoreCase)
    {
        ["email"] = () => new OpenApiString("user@example.com"),
        ["password"] = () => new OpenApiString("••••••••"),
        ["name"] = () => new OpenApiString("John Doe"),
        ["subject"] = () => new OpenApiString("Meeting Tomorrow"),
        ["body"] = () => new OpenApiString("Let's discuss the Q2 roadmap."),
        ["preview"] = () => new OpenApiString("Let's discuss the Q2..."),
        ["color"] = () => new OpenApiString("#3B82F6"),
        ["token"] = () => new OpenApiString("eyJhbGciOiJIUzI1NiIs..."),
        ["status"] = () => new OpenApiString("Pending"),
        ["firstname"] = () => new OpenApiString("John"),
        ["lastname"] = () => new OpenApiString("Doe"),
        ["username"] = () => new OpenApiString("johndoe"),
        ["city"] = () => new OpenApiString("New York"),
        ["country"] = () => new OpenApiString("US"),
        ["phone"] = () => new OpenApiString("+1-555-1234"),
        ["filename"] = () => new OpenApiString("report.pdf"),
        ["contenttype"] = () => new OpenApiString("application/pdf"),
        ["content_type"] = () => new OpenApiString("application/pdf"),
    };

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties is null)
            return;

        foreach (var prop in schema.Properties)
        {
            if (prop.Value.Example is not null)
                continue;

            if (prop.Value.Type != "string")
                continue;

            var cleanName = prop.Key.Replace("_", "");

            if (Examples.TryGetValue(cleanName, out var factory))
                prop.Value.Example = factory();

            if (cleanName.Contains("id", StringComparison.OrdinalIgnoreCase) && cleanName.Length <= 6)
                prop.Value.Example = new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        }
    }
}
