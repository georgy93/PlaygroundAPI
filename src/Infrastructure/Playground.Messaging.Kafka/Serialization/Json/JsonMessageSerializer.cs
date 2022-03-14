namespace Playground.Messaging.Kafka.Serialization.Json;

using System.Text;
using System.Text.Json;

public class JsonMessageSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
}