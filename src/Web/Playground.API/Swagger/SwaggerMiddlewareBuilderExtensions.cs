namespace Playground.API.Swagger
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;

    public static class SwaggerMiddlewareBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IConfiguration configuration)
        {
            var swaggerSettings = configuration
                .GetSection(nameof(SwaggerSettings))
                .Get<SwaggerSettings>();

            return app
                .UseSwagger(opt =>
                {
                    opt.RouteTemplate = swaggerSettings.JsonRoute;
                })
                .UseSwaggerUI(opt =>
                {
                    opt.SwaggerEndpoint(swaggerSettings.UIEndpoint, swaggerSettings.Description);
                });
        }
    }
}