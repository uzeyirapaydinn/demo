using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using QuickCode.Demo.Common;
using QuickCode.Demo.Common.Controllers;
using QuickCode.Demo.Common.Helpers;
using QuickCode.Demo.Common.Model;
using QuickCode.Demo.Common.Nswag.Extensions;
using QuickCode.Demo.EventListenerService;
using QuickCode.Demo.EventListenerService.Models;
using Serilog;

Dictionary<string, string> environmentVariableConfigMap = new()
{
    { "READ_CONNECTION_STRING", "ConnectionStrings:ReadConnection" },
    { "WRITE_CONNECTION_STRING", "ConnectionStrings:WriteConnection" },
    { "ELASTIC_CONNECTION_STRING", "ConnectionStrings:ElasticConnection" },
    { "USERMANAGERMODULEAPIKEY" , "QuickCodeApiKeys:UserManagerModuleApiKey" },
	{ "EMAILMANAGERMODULEAPIKEY" , "QuickCodeApiKeys:EmailManagerModuleApiKey" },
	{ "SMSMANAGERMODULEAPIKEY" , "QuickCodeApiKeys:SmsManagerModuleApiKey" },
	{ "EMAILSENDERMODULEAPIKEY" , "QuickCodeApiKeys:EmailSenderModule" },
	{ "APIKEY" , "AppSettings:ApiKey" }
};

var builder = WebApplication.CreateBuilder(args);
const string inputJsonContent = """
                                {
                                   "RequestInfo": {
                                      "Path": "/api/quick-code-module/db-types/mssql",
                                      "Method": "PUT",
                                      "Headers": {
                                         "Accept": "application/json",
                                         "Host": "quickcode-quickcode-gateway-api",
                                         "Authorization": "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJmOWJmZmEwOS1kYWU3LTRmZTItYTE0NS04OTExMGJiODQ2MTEiLCJlbWFpbCI6InV6ZXlpcmFwYXlkaW5AZ21haWwuY29tIiwiUGVybWlzc2lvbkdyb3VwSWQiOiIxIiwianRpIjoiYzg2MzdjMzQtZDZhNC00MDMyLWEwYjAtOGU5MWFkYzhlNjA5IiwiZXhwIjoxNzMwMzY1MzEyLCJpc3MiOiJodHRwczovL3F1aWNrY29kZS1hcGkucXVpY2tjb2RlLm5ldCIsImF1ZCI6InF1aWNrY29kZS1xdWlja2NvZGUtY2xpZW50LWlkIn0.TY4kaTHxRVZnnmiTi504kx5zWQ3TPE9hhPyvEqUr-6w",
                                         "Content-Type": "application/json",
                                         "traceparent": "00-f7e1f1ba4dadc720456b015f6fe202d0-30b1e7060b038df3-00",
                                         "Content-Length": "126",
                                         "X-Api-Key": "721de9b0-0def-41af-aa4e-66e454c92190"
                                      },
                                      "Body": {
                                         "key": "mssql",
                                         "name": "Microsoft SQL Server",
                                         "description": "Microsoft SQL Server",
                                         "iconUrl": "~/img/skins/sql_server_logo.png"
                                      }
                                   },
                                   "ResponseInfo": {
                                      "StatusCode": 200,
                                      "Headers": {
                                         "Content-Type": "application/json; charset=utf-8",
                                         "Date": "Thu, 31 Oct 2024 08:58:41 GMT",
                                         "Server": "Kestrel"
                                      },
                                      "Body": {
                                         "value": true
                                      }
                                   },
                                   "OrderId": 5,
                                   "ExceptionMessage": null,
                                   "ElapsedMilliseconds": 7846,
                                   "Timestamp": "2024-10-31T08:58:44.8656194Z"
                                }
                                """;

const string yamlContent = """
                           name: 'Order Processing Workflow'
                           version: '1.0.0'
                           description: 'A workflow for processing customer orders'

                           variables:
                             retryCount:
                               type: 'int'
                               value: '3'
                             retryType:
                               type: 'string'
                               value: 'hello'

                           steps:
                             validateOrder:
                               url: '{{QuickCodeClients.QuickCodeModuleApi}}/api/quick-code-module/db-types'
                               method: 'POST'
                               headers:
                                 X-Api-Key: '{{QuickCodeApiKeys.QuickCodeModuleApiKey}}'
                               body: 
                                 key: 'string'
                                 name: 'string'
                                 description: 'string'
                                 iconUrl: 'string'
                               timeoutSeconds: 30
                               onSuccess:
                                 - condition: 'validateOrder.statusCode == 200'
                                   action: 'processPayment'
                                 - condition: 'response.isValid == true'
                                   action: 'processPayment'
                                 - condition: 'default'
                                   action: 'handleInvalidOrder'

                           """;

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
Log.Information($"Started({environmentName})...");
builder.Configuration.UpdateConfigurationFromEnv(environmentVariableConfigMap);

var useHealthCheck = builder.Configuration.GetSection("AppSettings:UseHealthCheck").Get<bool>();
var kafkaBootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers");

builder.Services.AddLogger(builder.Configuration);
Log.Information($"{builder.Configuration["Logging:ApiName"]} Started.");

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddNswagServiceClient(builder.Configuration, typeof(QuickCodeBaseApiController));

builder.Services.AddHostedService<DynamicKafkaBackgroundService>();
builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck("kafka", new KafkaHealthCheck(kafkaBootstrapServers!));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (useHealthCheck)
{
    app.UseHealthChecks("/hc", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("/", () => "Kafka Background Event Listener Service is running...")
    .WithOpenApi();

app.MapPost("/set-topic-refresh-interval", ( [FromBody] int seconds) => 
    {
        if (seconds > 30)
        {
            DynamicKafkaBackgroundService.SetTopicRefreshInterval(seconds);
        }

        return $"Topic refresh interval set to {DynamicKafkaBackgroundService.GetTopicRefreshInterval()} seconds";
    })
    .WithOpenApi();

app.MapPost("/set-topic-listener-interval", ( [FromBody] int seconds) => 
    {
        if (seconds > 30)
        {
            DynamicKafkaBackgroundService.SetTopicListenerInterval(seconds);
        }
 
        return $"Topic listen interval set to {DynamicKafkaBackgroundService.GetTopicListenerInterval()} seconds";
    })
    .WithOpenApi();

app.MapGet("/get-topic-refresh-interval", () => DynamicKafkaBackgroundService.GetTopicRefreshInterval())
    .WithOpenApi();

app.MapGet("/get-topic-listener-interval", () => DynamicKafkaBackgroundService.GetTopicListenerInterval())
    .WithOpenApi();


app.MapPost("/execute-workflow", async (HttpContext context, IHttpClientFactory httpClientFactory,
    IConfiguration configuration, ILogger<Program> logger) =>
{
    try
    {
        var executor = new WorkflowExecutor(yamlContent, inputJsonContent, httpClientFactory.CreateClient(), logger, configuration);
        var results = await executor.ExecuteWorkflow();

        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Workflow execution failed");
        return Results.Problem("Workflow execution failed", statusCode: 500);
    }
});


app.Run();
