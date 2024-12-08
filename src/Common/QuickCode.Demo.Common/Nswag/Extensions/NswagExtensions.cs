using System.Reflection;
using QuickCode.Demo.Common.Helpers;

namespace QuickCode.Demo.Common.Nswag.Extensions
{
    public static class NswagExtensions
    {
        public static void AddNswagServiceClients<T>(this IServiceCollection services, string baseUrl,
            bool addRetryHandler = false)
        {
            var baseClientType = typeof(T);
            services.AddNswagServiceClients(baseUrl, baseClientType, addRetryHandler);
        }

        private static void AddNswagServiceClients(this IServiceCollection services, string baseUrl,
            Type baseClientType, bool addRetryHandler)
        {
            var addHttpClientMethods = typeof(HttpClientFactoryServiceCollectionExtensions).GetMethods()
                .Where(i => i.Name.StartsWith("AddHttpClient"));
            var methods = addHttpClientMethods.Where(i =>
                i.GetGenericArguments().Count() == 2 && i.GetParameters().Count() == 3 &&
                i.GetParameters()[2].ParameterType == typeof(Action<IServiceProvider, HttpClient>)).ToArray();

            var types = baseClientType.Assembly.GetTypes()
                .Where(i => i.Namespace != null && i.Namespace.Equals(baseClientType.Namespace));
            BindServiceTypes(services, baseUrl, methods, types, addRetryHandler);
        }

        public static void AddNswagServiceClients<T>(this IServiceCollection services, IConfiguration configuration,
            string baseUrlConfigKey)
        {
            string baseUrl = configuration[baseUrlConfigKey] ?? "";
            services.AddNswagServiceClients<T>(baseUrl);
        }

        public static void AddNswagServiceClient(this IServiceCollection services, IConfiguration configuration,
            Type type, bool addRetryHandler = false,
            string namespacePrefix = "QuickCode.Demo.Common.Nswag.Clients.")
        {
            services.AddNswagServiceClient(configuration, type.Assembly, addRetryHandler, namespacePrefix);
        }

        public static void AddNswagServiceClient(this IServiceCollection services, IConfiguration configuration,
            Assembly assembly, bool addRetryHandler=false, string namespacePrefix = "QuickCode.Demo.Common.Nswag.Clients.")
        {
            var endsWith = ".Contracts";
            var clientNamespaces = assembly.GetTypes()
                .Where(i => i.Namespace != null && i.Namespace!.StartsWith(namespacePrefix)
                                                && i.Namespace!.EndsWith(endsWith))
                .Select(i => i.Namespace).Distinct();

            foreach (var clientNamespace in clientNamespaces)
            {
                var clientType = assembly
                    .GetTypes()
                    .FirstOrDefault(i => i.Namespace != null && i.Namespace == clientNamespace && i.IsInterface);
                var moduleName = clientNamespace!.Replace(namespacePrefix, string.Empty)
                    .Replace(endsWith, string.Empty);

                var configKey = $"QuickCodeClients:{moduleName}";
                var baseUrl = configuration[configKey] ?? "";
                services.AddNswagServiceClients(baseUrl, clientType!, addRetryHandler);
            }
        }

        public static void AddNswagServiceClients<T>(this WebApplicationBuilder builder, string baseUrlConfigKey, bool addRetryHandler)
        {
            var services = builder.Services;
            var baseUrl = builder.Configuration[baseUrlConfigKey];
            var baseClientType = typeof(T);
            var addHttpClientMethods = typeof(HttpClientFactoryServiceCollectionExtensions).GetMethods()
                .Where(i => i.Name.StartsWith("AddHttpClient"));
            var methods = addHttpClientMethods.Where(i =>
                i.GetGenericArguments().Count() == 2 && i.GetParameters().Count() == 3 &&
                i.GetParameters()[2].ParameterType == typeof(Action<IServiceProvider, HttpClient>)).ToArray();

            var types = baseClientType.Assembly.GetTypes()
                .Where(i => i.Namespace != null && i.Namespace.Equals(baseClientType.Namespace));
            BindServiceTypes(services, baseUrl!, methods, types, addRetryHandler);
        }

        private static void BindServiceTypes(IServiceCollection services, string baseUrl,
            IEnumerable<MethodInfo> methods, IEnumerable<Type> types, bool addRetryHandler)
        {
            foreach (var interfaceType in types.Where(i =>
                         i.Name.StartsWith("I", StringComparison.Ordinal) && i.IsInterface &&
                         i.Name.EndsWith("Client", StringComparison.Ordinal)))
            {
                var clientTypeName = interfaceType.FullName!.Replace("Contracts.I", "");
                var clientType = interfaceType.Assembly.GetType(clientTypeName);
                if (clientType != null)
                {
                    if (methods.Any())
                    {
                        var clientBaseUrl = $"{baseUrl}";
                        Action<IServiceProvider, HttpClient> action = (serviceProvider, config) =>
                        {
                            TryAddDefaultHeaders(serviceProvider, config);
                            config.BaseAddress = new Uri(clientBaseUrl);
                        };

                        var genMethod = methods.First().MakeGenericMethod(interfaceType, clientType);
                        var httpClientBuilder = (IHttpClientBuilder)genMethod.Invoke(services,
                            new object[] { services, clientType.FullName!, action })!;

                        if (addRetryHandler)
                        {
                            httpClientBuilder.AddHttpMessageHandler<RetryHandler>();
                        }

                    }
                }
            }
        }

        private static void TryAddDefaultHeaders(IServiceProvider serviceProvider, HttpClient client)
        {
            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();

            if (httpContextAccessor == null) return;

            var headers = httpContextAccessor.HttpContext?.Request.Headers;

            if (headers == null) return;

            if (httpContextAccessor.HttpContext!.User.Identity!.IsAuthenticated)
            {
                if (httpContextAccessor.HttpContext!.User.Claims.Any(i => i.Type.Equals("QuickCodeApiToken")))
                {
                    var claimAuthToken =
                        httpContextAccessor.HttpContext!.User.Claims.First(i => i.Type.Equals("QuickCodeApiToken"));
                    httpContextAccessor.HttpContext.Request.Headers.Authorization = $"Bearer {claimAuthToken!.Value}";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {claimAuthToken!.Value}");
                }

            }
        }
    }
}
