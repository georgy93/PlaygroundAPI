namespace Playground.Application.Common.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class BusinessExceptionContractResolver : DefaultContractResolver
{
    private readonly IReadOnlySet<string> _excludedProperties = new HashSet<string>
    {
        nameof(Exception.Data),
        nameof(Exception.HelpLink),
        nameof(Exception.HResult),
        nameof(Exception.Source),
        nameof(Exception.StackTrace),
        nameof(Exception.TargetSite)
    };

    public BusinessExceptionContractResolver()
    {
        IgnoreSerializableInterface = true;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var defaultProperties = base.CreateProperties(type, memberSerialization);

        return defaultProperties.Where(p => !_excludedProperties.Contains(p.PropertyName)).ToList();
    }
}