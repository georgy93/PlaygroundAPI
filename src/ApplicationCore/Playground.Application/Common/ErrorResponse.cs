namespace Playground.Application.Common
{
    using System;

    public record ErrorResponse(string ErrorCode, string Description, Exception Exception)
    {
        //[JsonProperty()]
        //public Exception Exception { get; init; }
    }
}