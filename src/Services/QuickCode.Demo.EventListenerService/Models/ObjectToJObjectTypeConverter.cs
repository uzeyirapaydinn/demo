using Newtonsoft.Json.Linq;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace QuickCode.Demo.EventListenerService.Models;

public class ObjectToJObjectTypeConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => type == typeof(JObject);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yamlObject = deserializer.Deserialize(parser);
        return yamlObject == null ? new JObject() : JObject.FromObject(yamlObject);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        throw new NotImplementedException();
    }
}