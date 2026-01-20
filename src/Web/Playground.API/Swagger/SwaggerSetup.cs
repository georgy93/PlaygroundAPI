namespace Playground.API.Swagger;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
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
                BearerFormat = "JWT",
                Name = HeaderNames.Authorization,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme

            });
            opt.AddSecurityRequirement(x =>
            {
                var securityScheme = new OpenApiSecurityScheme
                {
                    //Reference = new OpenApiReference
                    //{
                    //    Id = JwtBearerDefaults.AuthenticationScheme,
                    //    Type = ReferenceType.SecurityScheme
                    //}
                };

                return new OpenApiSecurityRequirement()/* { { securityScheme, Array.Empty<string>() } }*/;
            });
            opt.IncludeXmlComments(GetCommentsPath());

            /* <PropertyGroup>
                <GenerateDocumentationFile>true</GenerateDocumentationFile>
                <NoWarn>$(NoWarn);1591</NoWarn>
              </PropertyGroup>
            */
        })
        .AddSwaggerGenNewtonsoftSupport() // fix performance issue on swagger UI
        .AddSwaggerExamplesFromAssemblyOf<Startup>();


    private static string GetCommentsPath()
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        return xmlPath;
    }
}