namespace Playground.API.DTOs
{
    using Newtonsoft.Json;
    using System;

    // TODO: Move to API
    public record ErrorResponse
    {
        public string ErrorCode { get; init; }

        public string Description { get; init; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Exception Exception { get; init; }
    }
}