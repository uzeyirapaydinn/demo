using QuickCode.Demo.Common;

namespace QuickCode.Demo.Gateway.Extensions;

public static partial class HealthCheckExtensions
{
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var useHealthCheck = configuration.GetSection("AppSettings:UseHealthCheck").Get<bool>();
        var healthCheckSeconds = configuration.GetSection("AppSettings:HealthCheckSeconds").Get<int>();
        var kafkaBootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers");
        if (useHealthCheck)
        {
            services.AddHealthChecks()
                .AddCheck("kafka", new KafkaHealthCheck(kafkaBootstrapServers!));
            services.AddHealthChecksUI(settings =>
            {
                InMemoryConfigProvider.ReadProxyConfigFromFile();
                settings.SetEvaluationTimeInSeconds(healthCheckSeconds);
                settings.MaximumHistoryEntriesPerEndpoint(60);
                settings.SetApiMaxActiveRequests(1);

                foreach (var cluster in InMemoryConfigProvider.proxyConfig.Clusters
                             .Where(i => !i.ClusterId.Equals("auth-api")).OrderBy(i => i.ClusterId))
                {
                    settings.AddHealthCheckEndpoint($"{cluster.ClusterId} health checks",
                        $"{cluster.Destinations!.First().Value.Address}/hc");
                }
            }).AddInMemoryStorage();
        }

        return services;
    }
}
