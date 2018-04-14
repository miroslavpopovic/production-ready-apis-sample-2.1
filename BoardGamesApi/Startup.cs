using BoardGamesApi.Data;
using BoardGamesApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGamesApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGamesRepository, GamesRepository>();

            services.AddJwtBearerAuthentication(Configuration);

            services.AddMvcCore()
                .AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV")
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddApiVersioning(c =>
            {
                c.AssumeDefaultVersionWhenUnspecified = true;
                c.ReportApiVersions = true;
            });

            services.AddSwagger();
        }

        public void Configure(
            IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Inject our custom error handling middleware into ASP.NET Core pipeline
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseAuthentication();

            app.UseMiddleware<LimitingMiddleware>();

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger(); // http://localhost:49194/swagger/v1/swagger.json
            app.UseSwaggerUI(c =>
            {
                // build a swagger endpoint for each discovered API version
                foreach (var description in provider.ApiVersionDescriptions
                    .Distinct((x, y) => x.GroupName == y.GroupName))
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
                }
                c.RoutePrefix = string.Empty; // Serve as app root instead of /swagger
            });
        }
    }
}
