namespace Playground.API
{
    using Application;
    using Behavior.Middlewares;
    using Infrastructure;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Persistence;
    using Swagger;

    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                  .AddApplication()
                  .AddInfrastructure(Configuration)
                  .AddPersistence(Configuration)
                  .AddPresentation(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                app.UseHsts();
            }

            // The order of the middleware is considered so it must be paid attention!
            app.UseSwagger(Configuration)
               .UseHttpsRedirection()
               .UseStaticFiles()
               .UseRouting()
               .UseCors("AllowAll")
               .UseCustomHealthChecks("/health")
               .UseAuthentication()
               .UseAuthorization()
               .UseMiddleware<GlobalExceptionHandlingMiddleware>()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               });
        }

        #region One Startup different configurations for specific environments

        public void ConfigureDevelopmentServices2(IServiceCollection services)
        {
            _ = services;
            // if I use only one Startup.cs and I want to have different ConfigureServices method:
            // ConfigureDevelopmentServices
            // ConfigureStagingServices
            // ConfigureProductiontServices
        }

        // remove 2 from the name of the method and run in development
        public void ConfigureDevelopment2(IApplicationBuilder app)
        {
            _ = app;
            // if I use only one Startup.cs and I want to have different Configure method:
            // ConfigureDevelopment
            // ConfigureStaging
            // ConfigureProduction
        }
        #endregion
    }
}