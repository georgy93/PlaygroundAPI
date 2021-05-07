namespace Playground.API
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Swagger;

    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services) => services
            .AddCustomWebApi()
            .AddHttpContextAccessor()
            .AddSwagger();

        private static IServiceCollection AddCustomWebApi(this IServiceCollection services)
        {
            services.AddControllers();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                // if we use [ApiController] it will internally use its own ModelState filter
                // and we will not reach our custom Validation Filter
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                    //.AllowCredentials(); // causes exception
                    );
            });

            return services;
        }
    }
}