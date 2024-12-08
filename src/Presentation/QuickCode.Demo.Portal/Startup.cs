using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuickCode.Demo.Common;
using QuickCode.Demo.Common.Filters;
using QuickCode.Demo.Common.Controllers;
using QuickCode.Demo.Common.Helpers;
using QuickCode.Demo.Common.Nswag.Extensions;
using QuickCode.Demo.Portal.Helpers;
using QuickCode.Demo.Portal.Helpers.Authorization;
using QuickCode.Demo.Portal.Mapper;
using QuickCode.Demo.Portal.ViewEngines;

namespace QuickCode.Demo.Portal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogger(Configuration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = new PathString("/Login/Index");
                    options.AccessDeniedPath = new PathString("/Login/Index");
                });

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            
            services.AddResponseCompression();
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            
            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

            services.AddSingleton<ITempDataProvider, SessionStateTempDataProvider>();
            
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(ApiLogFilterAttribute));
            }).AddRazorRuntimeCompilation();
            
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfiles());
            });
            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            
            services.AddTransient<RetryHandler>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IPortalPermissionManager, PortalPermissionManager>();
            
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IRazorViewEngine, RazorViewEngine>();
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Permission", policyBuilder =>
                {
                    policyBuilder.Requirements.Add(new PermissionAuthorizationRequirement());
                });
            });
            
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });

            services.AddNswagServiceClient(Configuration, typeof(Startup), true);
            services.AddNswagServiceClient(Configuration, typeof(QuickCodeBaseApiController), true);
            
            services.Configure<HtmlHelperOptions>(o => o.ClientValidationEnabled = false);
            
            services.AddResponseCaching();
            
            services.AddRazorPages().AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();
            app.UseResponseCompression();
            
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("en-US")),
                SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("tr-TR") },
                SupportedUICultures = new List<CultureInfo> { new CultureInfo("en"), new CultureInfo("tr") }
            });

            app.UseRouting();
            app.UseResponseCaching();
            app.UseAuthorization();
            app.UseMiddleware<CustomExceptionHandlingMiddleware>();
            
            app.UseStatusCodePages(async context =>
            {
                var response = context.HttpContext.Response;
                var viewName = "Error";

                try
                {
                    await RenderViewToStringAsync(context.HttpContext, viewName, new Dictionary<string, object>()
                    {
                        { "StatusCode", response.StatusCode },
                        { "StatusDescription", context.HttpContext.Request.Path.Value },
                    });
                }
                catch (Exception)
                {
                    response.ContentType = "text/html";
                    await response.WriteAsync($"<h1>Error {response.StatusCode}</h1><p>Sorry, an error occurred while processing your request.</p>");
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static async Task RenderViewToStringAsync(HttpContext context, string viewName, Dictionary<string,object> model)
        {
            var serviceProvider = context.RequestServices;
            var actionContextAccessor = serviceProvider.GetRequiredService<IActionContextAccessor>();
            var viewEngine = serviceProvider.GetRequiredService<IRazorViewEngine>();
            var tempDataProvider = serviceProvider.GetRequiredService<ITempDataProvider>();
            var statusCode = model["StatusCode"].AsInt32();
            var actionContext = actionContextAccessor.ActionContext ?? new ActionContext
            {
                HttpContext = context,
                RouteData = context.GetRouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            var viewEngineResult = viewEngine.FindView(actionContext, viewName, false);
            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException($"Could not find view: {viewName}");
            }

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                ["SessionData"] = context.User.Identity!.IsAuthenticated ? context.Session.GetString("MenuItems") : "",
                ["Layout"] = context.User.Identity!.IsAuthenticated ? "_Layout" : "_LayoutError",
                ["ErrorCode"] = statusCode.ToString()
            };
            
            SetErrorMessages(statusCode, viewData);

            await using var writer = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                viewEngineResult.View,
                viewData,
                new TempDataDictionary(context, tempDataProvider),
                writer,
                new HtmlHelperOptions()
            );

            await viewContext.View.RenderAsync(viewContext);
            var htmlContent = writer.ToString();

            await context.Response.WriteAsync(htmlContent);
        }

        private static void SetErrorMessages(int statusCode, ViewDataDictionary viewData)
        {
            string errorMessage = "Unknown Error";
            string errorDescription = "An unknown error occurred.";

            switch (statusCode)
            {
                case 400:
                    errorMessage = "Bad Request";
                    errorDescription = "The server could not understand the request due to invalid syntax.";
                    break;
                case 401:
                    errorMessage = "Unauthorized Access";
                    errorDescription = "Your session has expired or you are not authorized to view this page.";
                    break;
                case 403:
                    errorMessage = "Forbidden";
                    errorDescription = "You do not have permission to perform this action.";
                    break;
                case 404:
                    errorMessage = "Page Not Found";
                    errorDescription = "The page you are looking for could not be found.";
                    break;
                case 500:
                    errorMessage = "Server Error";
                    errorDescription = "An unexpected error occurred on the server.";
                    break;
            }

            viewData["ErrorMessage"] = errorMessage;
            viewData["ErrorDescription"] = errorDescription;
            viewData["ErrorIcon"] = $"ErrorIcon{statusCode}.png";
        }
    }
}