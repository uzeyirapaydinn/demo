namespace QuickCode.Demo.SmsManagerModule.Api.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using QuickCode.Demo.SmsManagerModule.Application.Interfaces.Repositories;
    using QuickCode.Demo.SmsManagerModule.Persistence.Contexts;
        
    public static class ExtensionLibrary
    {
        #region DI Registration
        public static IServiceCollection AddQuickCodeRepositories(this IServiceCollection services)
        {
            var repoInterfaces = typeof(IBaseRepository<>).Assembly.GetTypes()
                .Where(i => i.Name.EndsWith("Repository") && i.IsInterface);
                
            foreach (var interfaceType in repoInterfaces)
            {
                var implementationType = typeof(WriteDbContext).Assembly.GetTypes()
                    .FirstOrDefault(i => i.Name == interfaceType.Name[1..])
                    ?? throw new InvalidOperationException($"Implementation not found for {interfaceType.Name}");
                    
                services.AddScoped(interfaceType, implementationType);
            }
            
            return services;
        }
        #endregion

        #region String Extensions
        public static string GetParentDirectory(this string path) =>
            Directory.GetParent(path)?.FullName 
                ?? throw new ArgumentException("Invalid path", nameof(path));
        
        
        public static string UrlSlugify(this string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return string.Empty;
            
            var str = phrase.ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "", RegexOptions.Compiled);
            str = Regex.Replace(str, @"\s+", " ", RegexOptions.Compiled).Trim();
            str = str[..Math.Min(str.Length, 45)].Trim();
            str = Regex.Replace(str, @"\s", "-", RegexOptions.Compiled);
            return str;
        }

        public static string ReplaceRegEx(this string obj, string pattern, string replacement, 
            RegexOptions regexOptions = RegexOptions.IgnoreCase) =>
            Regex.Replace(obj, pattern, replacement, regexOptions);

        public static bool IsNullOrEmpty(this string obj) => 
            string.IsNullOrEmpty(obj);

        public static string ClearPhoneText(this string obj) =>
            obj.Replace("(", "")
               .Replace(")", "")
               .Replace(" ", "")
               .Replace("-", "");

        public static IEnumerable<string> SplitCamelCase(this string source)
        {
            const string pattern = @"[A-Z][a-z]*|[a-z]+|\d+";
            return Regex.Matches(source, pattern, RegexOptions.Compiled)
                       .Select(m => m.Value);
        }

        public static string SplitCamelCaseToString(this string source) =>
            string.Join(" ", source.SplitCamelCase());

        public static string AsSplitCapitalWithUnderline(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            
            var parts = text.SplitCamelCase().ToArray();
            return string.Join("_", parts.Select(p => p.ToUpperInvariant()));
        }
        #endregion

        #region Type Conversion Extensions
        public static int AsInt32(this object obj) =>
            Convert.ToInt32(obj);

        public static long AsInt64(this object obj) =>
            Convert.ToInt64(obj);

        public static double AsDouble(this object obj)
        {
            if (obj is string strValue)
            {
                var numberFormat = new NumberFormatInfo
                {
                    NumberDecimalSeparator = strValue.Contains(",") ? "," : "."
                };
                return Convert.ToDouble(strValue, numberFormat);
            }
            
            return Convert.ToDouble(obj);
        }

        public static DateTime AsDateTime(this object obj) =>
            Convert.ToDateTime(obj);

        public static bool AsBoolean(this object obj) =>
            Convert.ToBoolean(obj);

        public static string AsString(this object obj) =>
            obj?.ToString() ?? string.Empty;
        #endregion

        #region JSON Extensions
        public static string JsonSerializeObject(this object data)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = { 
                    new JavaScriptDateTimeConverter(),
                    new StringEnumConverter { AllowIntegerValues = true }
                },
                ContractResolver = new DynamicContractResolver(typeof(byte[])),
                NullValueHandling = NullValueHandling.Include,
                Formatting = Newtonsoft.Json.Formatting.Indented
            };
            
            return JsonConvert.SerializeObject(data, data.GetType(), settings);
        }

        public static object JsonDeserializeObject(this string objectString, Type returnType)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = { new JavaScriptDateTimeConverter() },
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Newtonsoft.Json.Formatting.Indented
            };
            
            return JsonConvert.DeserializeObject(objectString, returnType, settings)
                ?? throw new JsonSerializationException("Deserialization resulted in null object");
        }
        #endregion

        #region DataRow Extensions
        public static T GetValue<T>(this DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                throw new ArgumentException($"Column {columnName} not found", nameof(columnName));

            var value = row[columnName];
            if (value == DBNull.Value)
                return default!;

            return (T)Convert.ChangeType(value, typeof(T));
        }
        #endregion
    }
}