namespace Playground.Persistence.Mongo.EntitiesConfiguration
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.IdGenerators;
    using MongoDB.Bson.Serialization.Serializers;

    internal class MongoEntityConfiguration : IEntityConfiguration
    {
        public void Apply()
        {
            //BsonClassMap.RegisterClassMap<MongoEntity>(cm =>
            //{
            //    cm.AutoMap();
            //    cm.MapIdMember(c => c.Id)
            //        .SetIdGenerator(StringObjectIdGenerator.Instance)
            //        .SetSerializer(new StringSerializer(BsonType.ObjectId));
            //});
        }
    }
}