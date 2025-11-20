namespace Playground.Persistence.Mongo;

internal static class MongoCollectionNamesHelper
{
    private static readonly Dictionary<Type, string> _collectionNames = [];

    public static string GetCollectionName<TEntity>()
    {
        var type = typeof(TEntity);

        return _collectionNames.TryGetValue(type, out var collectionName)
            ? collectionName
            : type.Name + "s";
    }
}