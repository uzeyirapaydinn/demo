using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Model;

namespace QuickCode.Demo.Gateway.Messaging;

public static class Extensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
        => services
            .AddSingleton<IMessagePublisher, DefaultMessagePublisher>()
            .AddSingleton<IMessageSubscriber, DefaultMessageSubscriber>();


    public record OpenApiSpecOptions(string Endpoint, string Spec, Dictionary<string,string> PathOptions);


    public static void MapOpenApiSpecs(this IEndpointRouteBuilder endpoints, List<ClusterConfig> clusters, List<RouteConfig> routes, Dictionary<string, OpenApiSpecOptions> gatewaySwaggerSpecs)
    { 
        if (clusters != null)
        {
            foreach (var cluster in clusters)
            {
                if(gatewaySwaggerSpecs.ContainsKey(cluster.ClusterId))
                {
                    endpoints.MapOpenApiSpec(routes, gatewaySwaggerSpecs[cluster.ClusterId], cluster.ClusterId);
                }
            }
        }
    }


    private static OpenApiDocument FilterOpenApiDocument(List<RouteConfig> routesConfig, ClusterState cluster, OpenApiSpecOptions options, OpenApiDocument document)
    {
        // create new paths
        var rewrite = new OpenApiPaths();

        // map existing path
        var routes = routesConfig.Where(p => p.ClusterId == cluster.ClusterId);
        var hasCatchAll = routes != null && routes.Any(p => p.Match.Path!.Contains("**catch-all"));


        foreach (var path in document.Paths)
        {
            var rewritedPath = path.Key;
            foreach (var o in options.PathOptions)
            {
                rewritedPath = rewritedPath.Replace(o.Key, o.Value, StringComparison.InvariantCultureIgnoreCase);
            }

            // there is a catch all, all route are elligible 
            // or there is a route that match without any operation methods filtering
            if (hasCatchAll || routes!.Any(p => p.Match.Path!.Equals(rewritedPath) && p.Match.Methods == null))
            {
                rewrite.Add(rewritedPath, path.Value);
            }
            else
            {
                // there is a route that match
                var routeThatMatchPath = routes!.Any(p => p.Match.Path!.Equals(rewritedPath));
                if (routeThatMatchPath)
                {
                    // filter operation based on yarp config
                    var operationToRemoves = new List<OperationType>();
                    foreach (var operation in path.Value.Operations)
                    {
                        // match route and method
                        var hasRoute = routes!.Any(
                            p => p.Match.Path!.Equals(rewritedPath) && p.Match.Methods!.Contains(operation.Key.ToString().ToUpperInvariant())
                        );

                        if (!hasRoute)
                        {
                            operationToRemoves.Add(operation.Key);
                        }
                    }

                    // remove operation routes
                    foreach (var operationToRemove in operationToRemoves)
                    {
                        path.Value.Operations.Remove(operationToRemove);
                    }

                    // add path if there is any operation
                    if (path.Value.Operations.Any())
                    {
                        rewrite.Add(rewritedPath, path.Value);
                    }
                }
            }
        }

        // apply specific tagging for each route (optional!)
        //foreach (var path in rewrite)
        //{
        //    foreach (var operation in path.Value.Operations)
        //    {
        //        operation.Value.Tags = new List<OpenApiTag> {
        //        new OpenApiTag() { Name = cluster.ClusterId }
        //    };
        //    }
        //}

        // return new document
        return new OpenApiDocument(document)
        {
            Paths = rewrite
        };
    }
    /// <summary>
    /// Map open api spec based on cluster config
    /// </summary>
    public static void MapOpenApiSpec(this IEndpointRouteBuilder endpoints, List<RouteConfig> routes, OpenApiSpecOptions options, string clusterId)
    {
        endpoints.Map(options.Endpoint, async context =>
        {
            // get cluster configuration
            var configuration = endpoints.ServiceProvider.GetService<IProxyStateLookup>();
            configuration!.TryGetCluster(clusterId, out var cluster);

            // get first destination open api specs
            var client = new HttpClient();
            var root = cluster!.Destinations.First().Value.Model.Config.Address;
            var stream = await client.GetStreamAsync($"{root.TrimEnd('/')}/{options.Spec.TrimStart('/')}");

            // get open api documents
            var document = new OpenApiStreamReader().Read(stream, out var diagnostic);

            // get an api document rewritten and filtered
            // according to the route configuration that matches the cluster
            var specifications = FilterOpenApiDocument(routes, cluster,options, document);

            // write the new specifications as a response
            var result = specifications.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);
            
            result = result.Replace("\"ApiKey\": [ ]", "\"Bearer\": [ ]");
            result = result.Replace("\"name\": \"X-Api-Key\",", "\"bearerFormat\": \"Bearer\",");
            result = result.Replace("\"in\": \"header\"", "\"scheme\": \"Bearer\"");
            result = result.Replace("\"ApiKey\": {", "\"Bearer\": {");
            result = result.Replace("\"type\": \"apiKey\"", "\"type\": \"http\"");
            
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(result);
        });
    }

    public static void MapGetSwaggerForYarp(this IEndpointRouteBuilder endpoints, List<ClusterConfig> clusters, List<RouteConfig> routes, Dictionary<string,GatewaySwaggerSpec> gatewaySwaggerSpecs )
    {

        if (clusters != null)
        {
            foreach (var cluster in clusters)
            {
                if(gatewaySwaggerSpecs.ContainsKey(cluster.ClusterId))
                {
                    endpoints.MapSwaggerSpecs(routes, cluster, gatewaySwaggerSpecs[cluster.ClusterId]);
                }
            }
        }
    }

    public static void MapSwaggerSpecs(this IEndpointRouteBuilder endpoints, List<RouteConfig> config, ClusterConfig cluster, GatewaySwaggerSpec swagger)
    {
        endpoints.Map(swagger.Endpoint, async context => {
            var client = new HttpClient();
            var stream = await client.GetStreamAsync(swagger.Spec);

            var document = new OpenApiStreamReader().Read(stream, out var diagnostic);
            var rewrite = new OpenApiPaths();

            // map existing path
            var routes = config.Where(p => p.ClusterId == cluster.ClusterId);
            var hasCatchAll = routes != null && routes.Any(p => p.Match.Path!.Contains("**catch-all"));

            foreach (var path in document.Paths)
            {

                var rewritedPath = path.Key.Replace(swagger.TargetPath, swagger.OriginPath);

                // there is a catch all, all route are elligible 
                // or there is a route that match without any operation methods filtering
                if (hasCatchAll || routes!.Any(p => p.Match.Path!.Equals(rewritedPath) && p.Match.Methods == null))
                {
                    rewrite.Add(rewritedPath, path.Value);
                }
                else
                {
                    // there is a route that match
                    var routeThatMatchPath = routes!.Any(p => p.Match.Path!.Equals(rewritedPath));
                    if (routeThatMatchPath)
                    {
                        // filter operation based on yarp config
                        var operationToRemoves = new List<OperationType>();
                        foreach (var operation in path.Value.Operations)
                        {
                            // match route and method
                            var hasRoute = routes!.Any(
                                p => p.Match.Path!.Equals(rewritedPath) && p.Match.Methods!.Contains(operation.Key.ToString().ToUpperInvariant())
                            );

                            if (!hasRoute)
                            {
                                operationToRemoves.Add(operation.Key);
                            }
                        }

                        // remove operation routes
                        foreach (var operationToRemove in operationToRemoves)
                        {
                            path.Value.Operations.Remove(operationToRemove);
                        }

                        // add path if there is any operation
                        if (path.Value.Operations.Any())
                        {
                            rewrite.Add(rewritedPath, path.Value);
                        }
                    }
                }
            }

            document.Paths = rewrite;

            var result = document.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);
            await context.Response.WriteAsync(
                result
            );
        });
    }
}



static class ResultsExtensions
{
    public static IResult Html(this IResultExtensions resultExtensions, string html)
    {
        ArgumentNullException.ThrowIfNull(resultExtensions);

        return new HtmlResult(html);
    }
}

class HtmlResult : IResult
{
    private readonly string _html;

    public HtmlResult(string html)
    {
        _html = html;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = MediaTypeNames.Text.Html;
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(_html);
        return httpContext.Response.WriteAsync(_html);
    }
}

public class GatewaySwaggerSpec
{
    public string Endpoint { get; set; } = default!;
    public string Spec { get; set; } = default!;
    public string OriginPath { get; set; } = default!;
    public string TargetPath { get; set; } = default!;
}