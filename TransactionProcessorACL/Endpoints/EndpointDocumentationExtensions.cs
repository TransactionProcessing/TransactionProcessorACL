using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi;

namespace TransactionProcessorACL.Endpoints;

internal static class EndpointDocumentationExtensions
{
    public static RouteHandlerBuilder DocumentJsonRequestExample(
        this RouteHandlerBuilder endpoint,
        string exampleJson)
    {
        return endpoint.AddOpenApiOperationTransformer((operation, _, _) =>
        {
            if (operation.RequestBody?.Content.TryGetValue("application/json", out OpenApiMediaType? mediaType) == true)
                mediaType.Example = JsonNode.Parse(exampleJson);

            return Task.CompletedTask;
        });
    }

    public static RouteHandlerBuilder DocumentJsonResponseExample(
        this RouteHandlerBuilder endpoint,
        int statusCode,
        string exampleJson)
    {
        return endpoint.AddOpenApiOperationTransformer((operation, _, _) =>
        {
            if (operation.Responses?.TryGetValue(statusCode.ToString(), out IOpenApiResponse? response) == true &&
                response is OpenApiResponse openApiResponse &&
                openApiResponse.Content?.TryGetValue("application/json", out OpenApiMediaType? jsonMediaType) == true)
            {
                jsonMediaType.Example = JsonNode.Parse(exampleJson);
            }

            return Task.CompletedTask;
        });
    }

    public static RouteHandlerBuilder DocumentQueryParameter(
        this RouteHandlerBuilder endpoint,
        string name,
        string description,
        JsonSchemaType type = JsonSchemaType.String,
        object? exampleValue = null)
    {
        return endpoint.AddOpenApiOperationTransformer((operation, _, _) =>
        {
            operation.Parameters ??= [];
            OpenApiParameter? parameter = operation.Parameters.FirstOrDefault(candidate =>
                candidate.In == ParameterLocation.Query && candidate.Name == name) as OpenApiParameter;

            if (parameter is null)
            {
                parameter = new OpenApiParameter
                {
                    Name = name,
                    In = ParameterLocation.Query
                };
                operation.Parameters.Add(parameter);
            }

            parameter.Description = description;
            parameter.Required = true;
            parameter.Schema ??= new OpenApiSchema { Type = type };
            if (exampleValue is not null)
                parameter.Example = JsonSerializer.SerializeToNode(exampleValue);
            return Task.CompletedTask;
        });
    }
}
