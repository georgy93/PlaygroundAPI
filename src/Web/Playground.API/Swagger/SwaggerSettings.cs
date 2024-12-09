namespace Playground.API.Swagger;

public record SwaggerSettings
{
    public string Description { get; init; }

    public string JsonRoute { get; init; }

    public string Title { get; init; }

    public string UIEndpoint { get; init; }

    public string Version { get; init; }
}