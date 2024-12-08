using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Newtonsoft.Json;

namespace QuickCode.Demo.Common.Helpers;

    public class RetryHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private const int MaxRetries = 3;
        private readonly HttpClient _httpClient;

        public RetryHandler(IHttpContextAccessor httpContextAccessor, HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        private async Task UpdateHttpContextWithNewTokens(AccessTokenResponse newTokens)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var identity = httpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    var removeClaimList = new string[]
                        { "QuickCodeApiToken", "QuickCodeApiTokenExpiresIn", "RefreshToken" };
                    foreach (var claimName in removeClaimList)
                    {
                        var removeClaims = identity.FindFirst(claimName);
                        if (removeClaims != null)
                            identity.RemoveClaim(removeClaims);
                    }

                    identity.AddClaim(new Claim("QuickCodeApiToken", newTokens.AccessToken));
                    identity.AddClaim(new Claim("QuickCodeApiTokenExpiresIn", newTokens.ExpiresIn.ToString()));
                    identity.AddClaim(new Claim("RefreshToken", newTokens.RefreshToken));
                    
                    httpContext.User = new ClaimsPrincipal(identity);
                    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, httpContext.User,
                        new AuthenticationProperties
                        {
                            IsPersistent = false,
                            AllowRefresh = false,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                        });
                }
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var retriesCounter = 0;
            HttpResponseMessage response = null!;
            do
            {
                if (_httpContextAccessor.HttpContext!.User.Identity!.IsAuthenticated &&
                    _httpContextAccessor.HttpContext!.User.Claims.Any(i => i.Type.Equals("QuickCodeApiToken")))
                {
                    var claimAuthToken =
                        _httpContextAccessor.HttpContext!.User.Claims.First(i => i.Type.Equals("QuickCodeApiToken"));
                    _httpContextAccessor.HttpContext.Request.Headers.Authorization = $"Bearer {claimAuthToken.Value}";
                }

                response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized &&
                    response.Headers.Contains("Token-Expired"))
                {
                    var claimAuthorization = _httpContextAccessor.HttpContext!.User.Claims
                        .FirstOrDefault(c => c.Type == "QuickCodeApiToken")?.Value;

                    if (!request.Headers.Authorization!.Parameter!.Equals(claimAuthorization))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", claimAuthorization);
                        continue;
                    }

                    var configKey = $"QuickCodeClients:UserManagerModuleApi";
                    var baseUrl = _configuration[configKey] ?? "";
                    var refreshToken = _httpContextAccessor.HttpContext!.User.Claims
                        .FirstOrDefault(c => c.Type == "RefreshToken")?.Value;

                    var requestUri = $"{baseUrl}api/auth/refresh";
                    var requestModel = new RefreshRequest()
                    {
                        RefreshToken = refreshToken!
                    };

                    var requestRefreshToken = new HttpRequestMessage(HttpMethod.Post, requestUri)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8,
                            "application/json")
                    };

                    var responseRefreshToken = await _httpClient.SendAsync(requestRefreshToken, cancellationToken);

                    if (responseRefreshToken.IsSuccessStatusCode)
                    {
                        var jsonResponse = await responseRefreshToken.Content.ReadAsStringAsync(cancellationToken);
                        var newTokens = JsonConvert.DeserializeObject<AccessTokenResponse>(jsonResponse);
                        if (newTokens != null) await UpdateHttpContextWithNewTokens(newTokens);
                    }
                }
                else
                {
                    break;
                }
            } while (MaxRetries > ++retriesCounter);

            return response;
        }
    }