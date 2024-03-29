﻿namespace Playground.Application.Common.Integration;

using Newtonsoft.Json;

public record IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime createDate)
    {
        Id = id;
        CreationDate = createDate;
    }

    [JsonProperty]
    public Guid Id { get; private init; }

    [JsonProperty]
    public DateTime CreationDate { get; private init; }
}