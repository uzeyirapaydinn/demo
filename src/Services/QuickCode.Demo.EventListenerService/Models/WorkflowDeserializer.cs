using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace QuickCode.Demo.EventListenerService.Models;

public static class WorkflowDeserializer
{
    public static Workflow ParseWorkflow(string yamlContent)
    {
        var builder = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance);

        var valueDeserializerBuilder = builder.BuildValueDeserializer();
        var deserializer = builder
            .WithTypeConverter(new ObjectToJObjectTypeConverter())
            .Build();

        return deserializer.Deserialize<Workflow>(yamlContent);
    }
}