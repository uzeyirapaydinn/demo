using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickCode.Demo.Common.Extensions;
using QuickCode.Demo.Common.Nswag;
using QuickCode.Demo.Common.Model;
using QuickCode.Demo.Common.Nswag.Clients.UserManagerModuleApi.Contracts;
using QuickCode.Demo.Gateway.HTTP;
using QuickCode.Demo.Gateway.KafkaProducer;

namespace QuickCode.Demo.Gateway.Extensions;
public static class KafkaHelper
{
    internal static async Task<bool> GetTokenIsValid(IServiceProvider services, string token)
    {
        var authenticationsClient = services.GetRequiredService<IAuthenticationsClient>();
        var isValidToken = !token.IsTokenExpired() && await authenticationsClient.ValidateAsync(token);
        return isValidToken;
    }

    static async Task<List<KafkaEventsGetKafkaEventsResponseDto>> GetKafkaEvents(IServiceProvider services,
        IMemoryCache memoryCache)
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        var kafkaEventsClient = services.GetRequiredService<IKafkaEventsClient>();

        return await memoryCache.GetOrAddAsync<List<KafkaEventsGetKafkaEventsResponseDto>>("KafkaEvents",
            async parameters => await GetAllKafkaEvents(configuration, kafkaEventsClient, parameters),
            TimeSpan.FromMinutes(1));
    }


    static void SetKafkaApiKeyToClients(IKafkaEventsClient kafkaEventsClient, string configUserManagerApiKey)
    {
        (kafkaEventsClient as ClientBase)!.SetApiKey(configUserManagerApiKey);
    }


    static async Task<List<KafkaEventsGetKafkaEventsResponseDto>> GetAllKafkaEvents(IConfiguration configuration,
        IKafkaEventsClient kafkaEventsClient, object[] parameters)
    {
        var apiKeyConfigValue = $"QuickCodeApiKeys:UserManagerModuleApiKey";
        var configApiKey = configuration.GetValue<string>(apiKeyConfigValue);
        SetKafkaApiKeyToClients(kafkaEventsClient, configApiKey!);
        var kafkaEvents = await kafkaEventsClient.GetKafkaEventsAsync();
        return kafkaEvents.ToList();
    }

    internal static async Task<KafkaEventsGetKafkaEventsResponseDto?> CheckKafkaEventExists(IServiceProvider services,
        IMemoryCache memoryCache, HttpContext context)
    {
        var allKafkaEvents = await GetKafkaEvents(services, memoryCache);

        var kafkaEvent = allKafkaEvents.Find(endpoint =>
            endpoint.Path.IsRouteMatch(context.Request.Path) &&
            endpoint.IsActive &&
            endpoint.HttpMethod.Equals(context.Request.Method, StringComparison.OrdinalIgnoreCase));

        if (kafkaEvent == null)
        {
            return null;
        }

        kafkaEvent = KafkaEventsGetKafkaEventsResponseDto.FromJson(kafkaEvent.ToJson());
        var eventKey = $"_{kafkaEvent.HttpMethod}".ToLowerInvariant();
        kafkaEvent!.TopicName = $"{kafkaEvent!.TopicName}{eventKey}".ToLowerInvariant();
        return kafkaEvent;
    }

    private static JObject ConvertToJsonObject(string json)
    {
        try
        {
            var body = JToken.Parse(json);
            
            if (body is JObject jsonObject)
            {
                return jsonObject;
            }

            return new JObject { ["return"] = body };
        }
        catch (JsonReaderException)
        {
            return new JObject { ["error"] = "Invalid JSON format", ["value"] = json };
        }
    }
    
    internal static async Task SendKafkaMessageIfEventExists(
        IServiceProvider services, IMemoryCache memoryCache, IKafkaProducerWrapper kafkaProducer,
        HttpContext context, Stopwatch stopwatch)
    {
        var kafkaEvent = await CheckKafkaEventExists(services, memoryCache, context);
        if (kafkaEvent is null) return;

        var requestBodyText = await context.TryGetRequestBodyAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();

        var kafkaMessage = new KafkaMessage
        {
            RequestInfo = new RequestInfo
            {
                Path = context.Request.Path,
                Method = context.Request.Method,
                Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                Body = ConvertToJsonObject(requestBodyText)
            },
            ResponseInfo = new ResponseInfo
            {
                StatusCode = context.Response.StatusCode,
                Headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                Body = ConvertToJsonObject(responseBodyText)
            },
            ElapsedMilliseconds = (int)stopwatch.ElapsedMilliseconds,
            Timestamp = DateTime.UtcNow
        };

        var key = GenerateKey(context);
        SendKafkaMessage(kafkaProducer, kafkaEvent.TopicName, key, kafkaMessage);
    }

    internal static async Task SendErrorKafkaMessage(
        IKafkaProducerWrapper kafkaProducer, string topic, HttpContext context, Stopwatch stopwatch, Exception ex)
    {
        var errorKafkaMessage = new KafkaMessage
        {
            RequestInfo = new RequestInfo
            {
                Path = context.Request.Path,
                Method = context.Request.Method,
                Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
            },
            ResponseInfo = new ResponseInfo
            {
                StatusCode = context.Response.StatusCode,
                Body = new JObject { ["message"] = "An error occurred." }
            },
            ExceptionMessage = ex.Message,
            ElapsedMilliseconds = (int)stopwatch.ElapsedMilliseconds,
            Timestamp = DateTime.UtcNow
        };

        SendKafkaMessage(kafkaProducer, topic, GenerateKey(context), errorKafkaMessage);
    }


    static string GenerateKey(HttpContext context)
    {
        var timestamp = DateTime.UtcNow.Ticks;
        return $"{context.Request.Method}|{context.Request.Path}|{timestamp}";
    }

    private static void SendKafkaMessage(IKafkaProducerWrapper kafkaProducer, string topic, string key, KafkaMessage message)
    {
        Task.Run(async () =>
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include 
                };
                
                var serializedMessage = JsonConvert.SerializeObject(message, settings);
                await kafkaProducer.ProduceAsync(topic, key, serializedMessage);
        
                Console.WriteLine($"Message sent to Kafka topic: {topic}, Key: {key}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Message sending to Kafka timed out. Topic: {topic}, Key: {key}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send message to Kafka. Topic: {topic}, Key: {key}, Error: {ex.Message}");
            }
        });
    }
}