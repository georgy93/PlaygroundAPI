namespace Playground.API.Swagger.Examples
{
    using Application.Common;
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