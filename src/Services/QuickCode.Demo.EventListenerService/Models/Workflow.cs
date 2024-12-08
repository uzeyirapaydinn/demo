namespace QuickCode.Demo.EventListenerService.Models;

public class Workflow
{
    public string Name { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
    public Dictionary<string, VariableDefinition> Variables { get; set; }
    public Dictionary<string, Step> Steps { get; set; }
}