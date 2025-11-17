namespace Playground.API;

using Behavior.Filters;
using Behavior.Middlewares;
using Behavior.Settings;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidations;
using Newtonsoft.Json.Serialization;
using Swagger;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPresentation(IConfiguration config) => services
            .AddPresentationConfigurations(config)
            .AddCustomWebApi()
            .AddHttpContextAccessor()
            .AddSwagger(config);

        private IServiceCollection AddCustomWebApi()
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddControllers(opts =>
            {
                opts.Filters.Add<ModelValidationFilter>();
            })
            .AddNewtonsoftJson(jsonOptions =>
            {
                jsonOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            // https://docs.fluentvalidation.net/en/latest/upgrading-to-11.html
            services
                .AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssembly(typeof(BaseValidator<>).Assembly);

            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

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

        private IServiceCollection AddPresentationConfigurations(IConfiguration config) => services
            .Configure<ErrorHandlingSettings>(config.GetSection(nameof(ErrorHandlingSettings)))
            .Configure<ApiKeySettings>(config.GetSection(nameof(ApiKeySettings)));
    }
}