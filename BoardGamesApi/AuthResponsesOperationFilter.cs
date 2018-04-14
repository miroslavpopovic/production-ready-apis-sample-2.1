using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace BoardGamesApi
{
    /// <summary>
    /// Adds default responses to action methods with <see cref="AuthorizeAttribute"/>.
    /// </summary>
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        /// <inheritdoc/>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var authAttributes = context.ApiDescription
                .ControllerAttributes()
                .Union(context.ApiDescription.ActionAttributes())
                .OfType<AuthorizeAttribute>()
                .ToArray();

            if (authAttributes.Any())
                operation.Responses.Add(
                    "401", new Response { Description = "Unauthorized access to resource" });

            if (authAttributes.Any(x => x.Roles == "admin"))
                operation.Responses.Add(
                    "403", new Response { Description = "Forbidden - user not authorized to access resource" });
        }
    }
}
