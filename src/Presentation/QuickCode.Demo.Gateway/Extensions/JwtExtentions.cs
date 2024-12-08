using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace QuickCode.Demo.Gateway.Extensions;

public static class JwtExtensions
{
    public static Dictionary<string, string> ParseJwtPayload(this string jwtToken)
    {
        var parts = jwtToken.Split('.');
        var payloadData = new Dictionary<string, string>();

        if (parts.Length == 3)
        {
            var payload = parts[1];
            var payloadJson = Base64UrlDecode(payload);
            var jsonDocument = JsonDocument.Parse(payloadJson);

            foreach (var element in jsonDocument.RootElement.EnumerateObject())
            {
                payloadData[element.Name] = $"{element.Value}";
            }
        }
        else
        {
            Console.WriteLine("Invalid JWT format.");
        }

        return payloadData;
    }
    
    public static bool IsTokenExpired(this string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
        {
            throw new ArgumentException("Invalid JWT token.");
        }

        var tokenExpireTime = token.ParseJwtPayload().First(i=>i.Key.Equals(JwtRegisteredClaimNames.Exp)).Value;

        var expDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tokenExpireTime));
        var currentTime = DateTimeOffset.UtcNow;

        return expDateTimeOffset <= currentTime;
    }
    
    public static double GetTokenExpirationTime(this string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
        {
            throw new ArgumentException("Invalid JWT token.");
        }

        var tokenExpireTime = token.ParseJwtPayload().First(i=>i.Key.Equals(JwtRegisteredClaimNames.Exp)).Value;

        var expDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tokenExpireTime));
        var currentTime = DateTimeOffset.UtcNow;
        var differenceInSeconds = (expDateTimeOffset - currentTime).TotalSeconds + 5;
        return differenceInSeconds;
    }

    private static string Base64UrlDecode(string input)
    {
        var output = input.Replace('-', '+').Replace('_', '/');
        output = output.PadRight(output.Length + (4 - output.Length % 4) % 4, '=');
        var base64EncodedBytes = Convert.FromBase64String(output);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }
}
