using Newtonsoft.Json.Linq;

namespace QuickCode.Demo.EventListenerService.Models;

public class Step
{
    public string Url { get; set; }
    public string Method { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public JObject Body { get; set; }
    public int? TimeoutSeconds { get; set; }
    public List<ConditionalAction> OnSuccess { get; set; }
    public Dictionary<string, Step> Steps { get; set; }
    public List<string> DependsOn { get; set; }
    public string Condition { get; set; }
    public string Repeat { get; set; }
}