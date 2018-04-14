using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BoardGamesApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJwtBearerAuthentication(
            this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options =>
                    {
                        var tokenValidationParameters = new TokenValidationParameters
                        {
                            ValidIssuer = configuration["Tokens:Issuer"],
                            ValidAudience = configuration["Tokens:Issuer"],
                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(configuration["Tokens:Key"]))
                        };

                        options.TokenValidationParameters = tokenValidationParameters;
                    });
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", GetSwaggerDocInfo());

                c.OperationFilter<AuthResponsesOperationFilter>();

                var xmlDocumentationPath = Path.Combine(
                    PlatformServices.Default.Application.ApplicationBasePath, "BoardGamesApi.xml");
                c.IncludeXmlComments(xmlDocumentationPath);

                c.AddSecurityDefinition("bearer-token", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "bearer-token", new string[] { } }
                });
            });
        }

        private static Info GetSwaggerDocInfo()
        {
            return new Info
            {
                Title = "Board Games API v1",
                Version = "v1",
                Description = "A sample API for presentation purpose",
                TermsOfService = "Do whatever you like with it",
                Contact = new Contact
                {
                    Name = "Miroslav Popovic",
                    Url = "https://miroslavpopovic.com"
                },
                License = new License
                {
                    Name = "MIT",
                    Url = "https://opensource.org/licenses/MIT"
                }
            };
        }
    }
}
