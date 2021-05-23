namespace Playground.Messaging.Kafka.Serialization.Json
{
    using Confluent.Kafka;
    using System;
    using System.Text;
    using System.Text.Json;

    public class JsonMessageDeserializer<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context) => isNull
            ? default
            : JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
    }
}