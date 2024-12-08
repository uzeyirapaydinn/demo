using System;
using System.Text.Json;
using Yarp.ReverseProxy.Configuration;

namespace QuickCode.Demo.Gateway.Models
{

	public class ReverseProxyConfigModel
	{
		public ReverseProxyConfig ReverseProxy { get; set; } = new ReverseProxyConfig();

		public bool MergeReverseProxy(ReverseProxyConfig config)
        {
            AddClusters(config.Clusters);
            AddRoutes(config.Routes);
            return true;
        }

        public void ClearAll()
        {
            ReverseProxy.Routes.Clear();
            ReverseProxy.Clusters.Clear();
        }

        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true

            };

            var jsonValue = JsonSerializer.Serialize(ReverseProxy, options);
            return jsonValue;
        }

        public void ClearClusters()
        {
            ReverseProxy.Clusters.Clear();
        }

        public void ClearRoutes()
        {
            ReverseProxy.Routes.Clear();
        }

        public List<RouteConfig> Routes
        {
            get
            {
                return ReverseProxy.Routes.Select(i => i.Value).ToList();
            }
        }


        public List<ClusterConfig> Clusters
        {
            get
            {
                return ReverseProxy.Clusters.Select(i => i.Value).ToList();
            }
        }


        public bool AddRoute(RouteConfig route)
        {
            ReverseProxy.Routes.Add(route.RouteId, route);
            return true;
        }

        public bool RemoveRoute(string routeId)
        {
           return ReverseProxy.Routes.Remove(routeId);
        }

        public bool AddCluster(string clusterId, string destination, IReadOnlyDictionary<string, string>? metadata = null)
        {
            return AddCluster(clusterId, new string[] { destination }, metadata);
        }

        public bool AddCluster(string clusterId, string[] destinations, IReadOnlyDictionary<string, string>? metadata = null)
        {
            var destinationConfigs = new Dictionary<string, DestinationConfig>();
            var counter = 1;
            foreach (var destination in destinations)
            {
                destinationConfigs.Add($"destination{counter++}", new DestinationConfig() { Address = destination });
            }

            var clusterConfig = new ClusterConfig()
            {
                ClusterId = clusterId,
                Destinations = destinationConfigs,
                Metadata = metadata

            };

            ReverseProxy.Clusters[clusterId] = clusterConfig;
            return true;
        }

        public bool AddCluster(ClusterConfig cluster)
        {
            ReverseProxy.Clusters[cluster.ClusterId] = cluster;
            return true;
        }

        public bool AddRoutes(Dictionary<string, RouteConfig> routes)
		{
			foreach(var key in routes.Keys)
			{
                ReverseProxy.Routes[key] = routes[key];
            }

			return true;
		}

        public bool AddClusters(Dictionary<string, ClusterConfig> clusters)
        {
            foreach (var key in clusters.Keys)
            {
                ReverseProxy.Clusters[key] = clusters[key];
            }

            return true;
        }

    }

    public class ReverseProxyConfig
	{
		public Dictionary<string, RouteConfig> Routes { get; set; } = new Dictionary<string, RouteConfig>();
		public Dictionary<string, ClusterConfig> Clusters { get; set; } = new Dictionary<string, ClusterConfig>();
	}
}

