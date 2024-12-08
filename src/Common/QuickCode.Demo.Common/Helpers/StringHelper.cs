using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace QuickCode.Demo.Common.Helpers
{
    public static class StringHelper
	{
        public static string PascalToKebabCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(
                value,
                "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z0-9])",
                "-$1",
                RegexOptions.Compiled)
                .Trim()
                .ToLower();
        }
        
        public static string KebabCaseToPascal(this string value, string joinString = " ")
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return string.Join(joinString, value.Split("-").Select(i => i.FirstCharToUpper()));
        }
        
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };

        public static T GetHttpContextItem<T>(this IHttpContextAccessor contextAccessor)
        {
            var headerItem = (T)contextAccessor.HttpContext!.GetItem<T>();
            return headerItem;
        }

        public static void SetHttpContextItem<T>(this IHttpContextAccessor contextAccessor, T value )
        {
            contextAccessor.HttpContext!.SetItem<T>(value);
        }

        public static T GetItem<T>(this HttpContext httpContext)
        {
            var itemKey = typeof(T).Name;
            if (httpContext.Items.TryGetValue(itemKey, out var item))
            {
                var headerItem = (T)item!;
                return headerItem;
            }

            return default(T)!;
        }

        public static void SetItem<T>(this HttpContext httpContext, T value)
        {
            var itemKey = typeof(T).Name;
            httpContext.Items[itemKey] = value;
        }
        
        public static string? GetPathValueFromJson(this string json, string path)
        {
            var jToken = JToken.Parse(json);
            
            var token = jToken.SelectToken(path);

            return token?.ToString();
        }
        
        public static string? GetPathValue(this JObject jObject, string path)
        {
            var token = jObject.SelectToken(path);
            return token?.ToString();
        }
    }



}

