namespace Playground.Persistence.Mongo;

internal class MongoDbSettings
{
    public string DatabaseName { get; init; }

    public bool SubscribeToEvents { get; init; }
}