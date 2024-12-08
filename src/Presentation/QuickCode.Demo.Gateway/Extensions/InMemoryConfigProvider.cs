using QuickCode.Demo.Gateway.Models;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;
using static QuickCode.Demo.Gateway.Messaging.Extensions;
using CloudFlare.Client;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Enumerators;
using Task = System.Threading.Tasks.Task;
using CloudFlare.Client.Api.Users;
using System.Net;
using QuickCode.Demo.Common.Extensions;

namespace QuickCode.Demo.Gateway.Extensions
{
    public class InMemoryConfigProvider : IProxyConfigProvider, IHostedService, IDisposable
    {
        private Timer _timer = default!;
        private volatile InMemoryConfig _config = default!;
        public static WebApplication? app;
        public static ReverseProxyConfigModel proxyConfig = new();
        public static int IsClustersUpdatedFromConfig = -1;
        public static DateTime LastUpdateDateTime = DateTime.Now;
        public static Dictionary<string, OpenApiSpecOptions> swaggerMaps = new();
        private static string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";
        public IConfiguration _configuration { get; }

        public InMemoryConfigProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            ReadProxyConfigFromFile();

            PopulateConfig();
        }

        public static void ReadProxyConfigFromFile()
        {
            proxyConfig.ClearAll();
            Console.WriteLine($"environmentName : {environmentName}");
            Directory.GetFiles("ProxyConfigs").ToList().ForEach(configFile =>
            {
                var fileInfo = new FileInfo(configFile);
                var fileName = configFile;
                var fileEnv = fileInfo.Name.Split(".");


                if (fileEnv.Length == 2 || (fileEnv.Length == 3 && fileEnv[1].Equals(environmentName)))
                {
                    Console.WriteLine($"ProxyConfig file : {fileName}");
                    var fileContent = File.ReadAllText(fileName);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var reverseProxyItem = JsonSerializer.Deserialize<ReverseProxyConfigModel>(fileContent, options);
                    proxyConfig.MergeReverseProxy(reverseProxyItem!.ReverseProxy);

                }
            });
        }

        void UpdateSwaggerMaps()
        {
            Directory.GetFiles("SwaggerMaps").ToList().ForEach(configFile =>
            {
                var fileInfo = new FileInfo(configFile);
                var fileName = configFile;

                Console.WriteLine($"Swagger Map file : {fileName}");
                var fileContent = File.ReadAllText(fileName);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                InMemoryConfigProvider.swaggerMaps = JsonSerializer.Deserialize<Dictionary<string, OpenApiSpecOptions>>(fileContent)!;
                app!.MapOpenApiSpecs(InMemoryConfigProvider.proxyConfig.Clusters, InMemoryConfigProvider.proxyConfig.Routes, InMemoryConfigProvider.swaggerMaps);
            });
        }

        private bool UpdateClusters()
        {
            var projectName = typeof(InMemoryConfigProvider).Namespace!.Split(".")[1];
            if (IsClustersUpdatedFromConfig == 0)
            {
                ReadProxyConfigFromFile();
                foreach (var cluster in proxyConfig.Clusters)
                {
                    var routeId = $"{cluster.ClusterId}-swagger";
                    
                    try
                    {
                        proxyConfig.RemoveRoute(routeId);
                        proxyConfig.AddRoute(new RouteConfig()
                        {
                            ClusterId = cluster.ClusterId,
                            RouteId = routeId,
                            Match = new RouteMatch()
                            {
                                Path = $"/swagger/{cluster.ClusterId}/{{**catch-all}}",
                                Hosts = new List<string>()
                                {
                                    "localhost:6060",
                                    $"{projectName.ToLowerInvariant()}-gateway-7exu2rabtq-ew.a.run.app",
                                    $"{projectName.ToLowerInvariant()}-gateway.quickcode.net"
                                }
                            },
                            Transforms = new List<Dictionary<string, string>>()
                            {
                                new() { { "PathPattern", "/swagger/{**catch-all}" } }
                            },

                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ClusterId: {cluster.ClusterId}, Couldn't get ClusterPublicIp: {ex.ToString()}");

                    }
                }
            }

            UpdateSwaggerMaps();

            return true;
        }

        private void Update(object state)
        {
            if (IsClustersUpdatedFromConfig != -1) return;
            IsClustersUpdatedFromConfig = 0; 
            UpdateClusters();
            PopulateConfig();
            LastUpdateDateTime = DateTime.Now;
            IsClustersUpdatedFromConfig = 1;
        }

        private async Task<bool> IsValidUrlAsync(string url)
        {
            using var client = new HttpClient();
            var result = await client.GetAsync(new Uri(url));
            var statusCode = result.StatusCode;

            return statusCode switch
            {
                HttpStatusCode.Accepted => true,
                HttpStatusCode.OK => true,
                _ => false
            };
        }

        private void PopulateConfig()
        {
            var oldConfig = _config;
            _config = new InMemoryConfig(proxyConfig.Routes, proxyConfig.Clusters);
            oldConfig?.SignalChange();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }


        public IProxyConfig GetConfig() => _config;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(new TimerCallback(Update!), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }


        private class InMemoryConfig : IProxyConfig
        {
            private readonly CancellationTokenSource _cts = new CancellationTokenSource();

            public InMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
            {
                Routes = routes;
                Clusters = clusters;
                ChangeToken = new CancellationChangeToken(_cts.Token);
            }


            public IReadOnlyList<RouteConfig> Routes { get; }

            public IReadOnlyList<ClusterConfig> Clusters { get; }

            public IChangeToken ChangeToken { get; }

            internal void SignalChange()
            {
                _cts.Cancel();
            }

        }

    }
}
