using System.Net;
using System.Text;
using Polly;
using Polly.Extensions.Http;

namespace QuickCode.Demo.Gateway.HTTP;

public static class Extensions
{
    public static IServiceCollection AddHttpApiClient<TInterface, TClient>(this IServiceCollection services) 
        where TInterface : class where TClient : class, TInterface
    {
        services
            .AddHttpClient<TInterface, TClient>()
            .AddPolicyHandler(GetPolicy());
        
        return services;

        static IAsyncPolicy<HttpResponseMessage> GetPolicy()
            => HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.BadRequest)
                .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(Math.Pow(2, retry)));
    }

    public static async Task<string> TryGetRequestBodyAsync(this HttpContext context)
    {
        var buffer = new char[8192];
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var requestBody = new StringBuilder();

        int bytesRead;
        context.Request.Body.Position = 0;
        while ((bytesRead = await reader.ReadBlockAsync(buffer, 0, buffer.Length)) > 0)
        {
            requestBody.Append(buffer, 0, bytesRead);
        }

        if (bytesRead > 0)
        {
            context.Request.Body.Position = 0;
        }

        return requestBody.ToString();
    }
}