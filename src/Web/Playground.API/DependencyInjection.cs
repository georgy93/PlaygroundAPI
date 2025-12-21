namespace Playground.API;

using Behavior.Middlewares;
using Behavior.Settings;
using FluentValidation;
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
            services.AddExceptionHandler<ModelValidationHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services
                .AddControllers()
                .AddNewtonsoftJson(jsonOptions =>
                {
                    jsonOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            // https://docs.fluentvalidation.net/en/latest/upgrading-to-11.html
            services
                .AddValidatorsFromAssemblyContaining(typeof(BaseValidator<>));

            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

            return services
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