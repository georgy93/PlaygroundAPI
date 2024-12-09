namespace Playground.API.Swagger.Examples;

public class ErrorResponseExample : IExamplesProvider<ErrorResponse>
{
    public ErrorResponse GetExamples() => new()
    {
        ErrorCode = "InternalServerError",
        Description = "InternalServerError",
        Exception = null
    };
}