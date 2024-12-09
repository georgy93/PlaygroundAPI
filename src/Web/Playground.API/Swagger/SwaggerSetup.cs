﻿namespace Playground.API.Swagger;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Reflection;

public static class SwaggerSetup
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration) => services
        .AddSwaggerGen(opt =>
        {
            var swaggerSettings = configuration
                .GetSection(nameof(SwaggerSettings))
                .Get<SwaggerSettings>();

            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = swaggerSettings.Title,
                Description = swaggerSettings.Description,
                Version = swaggerSettings.Version
            });
            opt.ExampleFilters();
            opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the bearer scheme",
                Name = HeaderNames.Authorization,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
            opt.AddSecurityRequirement(CreateSecurityRequirement());
            opt.IncludeXmlComments(GetCommentsPath());

            /* <PropertyGroup>
                <GenerateDocumentationFile>true</GenerateDocumentationFile>
                <NoWarn>$(NoWarn);1591</NoWarn>
              </PropertyGroup>
            */
        })
        .AddSwaggerGenNewtonsoftSupport() // fix performance issue on swagger UI
        .AddSwaggerExamplesFromAssemblyOf<Startup>();

    private static OpenApiSecurityRequirement CreateSecurityRequirement()
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };

        return new() { { securityScheme, Array.Empty<string>() } };
    }

    private static string GetCommentsPath()
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        return xmlPath;
    }
}