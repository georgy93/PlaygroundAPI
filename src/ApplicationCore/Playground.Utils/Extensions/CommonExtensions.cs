namespace Playground.Utils.Extensions
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class CommonExtensions
    {
        public static string Stringify(this object obj, IContractResolver contractResolver = null) => JsonConvert
            .SerializeObject(obj, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = contractResolver ?? new DefaultContractResolver()
            });
    }
}