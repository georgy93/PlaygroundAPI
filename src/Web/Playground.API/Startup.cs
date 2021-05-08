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
        public void ConfigureServices(IServiceCollection services)
        {
            services
                  .AddApplication()
                  .AddInfrastructure(Configuration)
                  .AddPersistence(Configuration)
                  .AddPresentation(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // The order of the middleware is considered so it must be paid attention!
            app.UseCors("AllowAll")
               .UseSwagger(Configuration)
               .UseAuthentication()
               .UseRouting()
               .UseMiddleware<GlobalExceptionHandlingMiddleware>()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               });
        }
    }
}