namespace Playground.API.Swagger
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Net.Http.Headers;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.Filters;
    using System;
    using System.IO;
    using System.Reflection;

    public static class SwaggerSetup
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services) => services
            .AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Playground API",
                    Description = "This is just a playground application",
                    Version = "v1"
                });
                opt.ExampleFilters();
                opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
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
            .AddSwaggerExamplesFromAssemblyOf<Startup>();

        private static OpenApiSecurityRequirement CreateSecurityRequirement()
        {
            var securityScheme = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
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
}