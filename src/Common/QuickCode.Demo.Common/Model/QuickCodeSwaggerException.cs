namespace QuickCode.Demo.Common.Model;

public partial class QuickCodeSwaggerException : Exception
{
    public int StatusCode { get; private set; }

    public string Response { get; private set; }

    public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; }

    private static string GetExceptionMessage(string message, int statusCode, string response)
    {
        var responseLength = response.Length > 512 ? 512 : response.Length;
        return $"{message}\n\nStatus: {statusCode}\nResponse:\n{response[0..responseLength]}";
    }

    public QuickCodeSwaggerException(string message, int statusCode, string response,
        IReadOnlyDictionary<string, IEnumerable<string>> headers, System.Exception innerException)
        : base(GetExceptionMessage(message, statusCode, response), innerException)
    {
        StatusCode = statusCode;
        Response = response;
        Headers = headers;
    }

    public override string ToString()
    {
        return $"HTTP Response:\n\n{Response}\n\n{base.ToString()}";
    }
}

public partial class QuickCodeSwaggerException<TResult> : QuickCodeSwaggerException
{
    public TResult Result { get; private set; }

    public QuickCodeSwaggerException(string message, int statusCode, string response,
        IReadOnlyDictionary<string, IEnumerable<string>> headers, TResult result, Exception innerException)
        : base(message, statusCode, response, headers, innerException)
    {
        Result = result;
    }
}