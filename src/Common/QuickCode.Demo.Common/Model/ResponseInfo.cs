using Newtonsoft.Json.Linq;

namespace QuickCode.Demo.Common.Model;

public class ResponseInfo
{
    public int StatusCode { get; set; }
    public Dictionary<string, string> Headers { get; set; } = [];
    public JObject Body { get; set; } = default!;
}