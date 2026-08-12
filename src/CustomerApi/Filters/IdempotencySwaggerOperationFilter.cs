using CustomerApi.Web.Attributes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CustomerApi.Web.Filters;

public class IdempotencySwaggerOperationFilter : IOperationFilter
{
  //Filter pra colocar header de idempotencia no swagger pra facilitar testes
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    var attribute = context.MethodInfo?
      .GetCustomAttributes(typeof(IsIdempotentAttribute), inherit: true)
      .OfType<IsIdempotentAttribute>()
      .FirstOrDefault();

    if (attribute is null)
    {
      attribute = context.ApiDescription.ActionDescriptor.EndpointMetadata
        .OfType<IsIdempotentAttribute>()
        .FirstOrDefault();
    }

    if (attribute is null)
    {
      return;
    }

    var parameter = new OpenApiParameter
    {
      Name = attribute.HeaderName,
      In = ParameterLocation.Header,
      Required = true,
      Description = "Idempotency key gerada automaticamente. Use o mesmo valor para retentativas da mesma operação.",
      Schema = new OpenApiSchema
      {
        Type = "string",
        Example = new OpenApiString(Guid.NewGuid().ToString())
      }
    };

    operation.Parameters ??= [];
    operation.Parameters.Add(parameter);
  }
}
