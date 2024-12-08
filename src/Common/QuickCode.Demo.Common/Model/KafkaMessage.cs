namespace QuickCode.Demo.Common.Model;

public class KafkaMessage
{
    public RequestInfo RequestInfo { get; set; }
    public ResponseInfo ResponseInfo { get; set; }
    public string ExceptionMessage { get; set; } = default!;
    public int ElapsedMilliseconds { get; set; }
    public DateTime Timestamp { get; set; }
}