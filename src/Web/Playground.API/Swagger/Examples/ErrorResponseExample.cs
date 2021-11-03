namespace Playground.API.Swagger.Examples
{
    using DTOs;
    using Swashbuckle.AspNetCore.Filters;

    public class ErrorResponseExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples() => new()
        {
            ErrorCode = "InternalServerError",
            Description = "InternalServerError",
            Exception = null
        };
    }
}