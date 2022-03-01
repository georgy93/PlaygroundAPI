namespace Playground.Persistence.Mongo
{
    internal static class MongoCollectionNamesHelper
    {
        private static readonly IReadOnlyDictionary<Type, string> _collectionNames = new Dictionary<Type, string>()
        {
            // { typeof(MongoEntity), "MongoEntities" }
        };

        public static string GetCollectionName<TEntity>()
        {
            var type = typeof(TEntity);

            return _collectionNames.TryGetValue(type, out var collectionName)
                ? collectionName
                : type.Name + "s";
        }
    }
}