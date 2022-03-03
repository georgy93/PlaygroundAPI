namespace Playground.API.Swagger
{
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