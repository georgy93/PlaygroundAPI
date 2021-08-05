namespace Playground.API
{
    using Behavior.Filters;
    using Behavior.Settings;
    using FluentValidation.AspNetCore;
    using FluentValidations;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json.Serialization;
    using Swagger;

    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration config) => services
            .AddPresentationConfigurations(config)
            .AddCustomWebApi()
            .AddHttpContextAccessor()
            .AddSwagger();

        private static IServiceCollection AddCustomWebApi(this IServiceCollection services)
        {
            services.AddControllers(opts =>
            {
                //opts.EnableEndpointRouting = false;
                opts.Filters.Add<ModelValidationFilter>();
            })
            .AddNewtonsoftJson(jsonOptions =>
            {
                jsonOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddFluentValidation(mvcConfig =>
            {
                mvcConfig.RegisterValidatorsFromAssemblyContaining(typeof(BaseValidator<>));
            });

            return services
                 .Configure<ApiBehaviorOptions>(options =>
                 {
                     // if we use [ApiController] it will internally use its own ModelState filter
                     // and we will not reach our custom Validation Filter
                     options.SuppressModelStateInvalidFilter = true;
                 })
                 .AddCors(options =>
                 {
                     options.AddPolicy("AllowAll",
                         builder => builder
                             .AllowAnyOrigin()
                             .AllowAnyMethod()
                             .AllowAnyHeader()
                         //.AllowCredentials(); // causes exception
                         );
                 });
        }

        private static IServiceCollection AddPresentationConfigurations(this IServiceCollection services, IConfiguration config) => services
            .Configure<ErrorHandlingSettings>(config.GetSection(nameof(ErrorHandlingSettings)))
            .Configure<ApiKeySettings>(config.GetSection(nameof(ApiKeySettings)));
    }
}