namespace QuickCode.Demo.Common.Helpers;

using Serilog;
using System;

public static class EnvironmentHelper
{
    public static void UpdateConfigurationFromEnv(this IConfiguration configuration, Dictionary<string, string> envToConfigMap)
    {
        foreach (var (envKey, configKey) in envToConfigMap)
        {
            var envValue = Environment.GetEnvironmentVariable(envKey);
            if (string.IsNullOrEmpty(envValue))
            {
                continue;
            }

            configuration[configKey] = envValue;
            var updatedValue = configuration[configKey] ?? "null";
            Log.Information($"{configKey} env key: {envKey} updated to: {updatedValue}");
        }
    }
}