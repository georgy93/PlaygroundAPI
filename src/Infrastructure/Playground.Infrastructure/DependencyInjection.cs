namespace Playground.Infrastructure
{
    using Application.Interfaces;
    using Application.Interfaces.Gateways;
    using Authorization;
    using Identity;
    using Identity.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Polly;
    using Polly.Extensions.Http;
    using Refit;
    using RestMessaging.DelegatingHandlers;
    using Services;
    using Services.Background;
    using System;
    using System.Text;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) => services
            .AddMemoryCache()
            .AddCustomAuthentication(configuration)
            .AddCustomAuthorization()
            .AddServices()
            .AddBackgroundServices()
            .AddGateways();

        private static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            var jwtSettingsConfig = configuration.GetSection(nameof(JwtSettings));

            jwtSettingsConfig.Bind(jwtSettings);

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidationParameters;
            });

            return services
                .Configure<JwtSettings>(jwtSettingsConfig)
                .AddSingleton(tokenValidationParameters)
                .AddScoped<IIdentityService, IdentityService>();
        }

        private static IServiceCollection AddCustomAuthorization(this IServiceCollection services) => services
            .AddSingleton<IAuthorizationHandler, WorksForCompanyHandler>()
            .AddAuthorizationCore(options =>
            {
                options.AddPolicy(AuthorizationPolicies.MustWorkForMe, policy =>
                {
                    policy.AddRequirements(new WorksForCompanyRequirement("me.com"));
                });
            });

        private static IServiceCollection AddServices(this IServiceCollection services) => services
            .AddSingleton<IResponseCacheService, ResponseCacheInMemoryService>()
            .AddTransient<IDateTimeService, DateTimeService>();

        private static IServiceCollection AddBackgroundServices(this IServiceCollection services) => services
            .AddHostedService<SomeBgService>();

        private static IServiceCollection AddGateways(this IServiceCollection services)
        {
            services
                .AddRefitClient<ISomeServiceGateway>(new RefitSettings())
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri("https://www.githudb.com"); // error in link so that exception handlers are called
                    client.Timeout = TimeSpan.FromSeconds(3);
                })
                .AddHttpMessageHandler<RequestExceptionHandlingBehavior>()
                .AddHttpMessageHandler<RequestStatisticsBehavior>()
                .AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(60)))
                .AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(3, TimeSpan.FromSeconds(1))
                );

            return services
                .AddTransient<RequestExceptionHandlingBehavior>()
                .AddTransient<RequestStatisticsBehavior>();
        }
    }
}