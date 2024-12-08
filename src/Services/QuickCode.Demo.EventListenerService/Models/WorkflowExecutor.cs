using System.Text.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using QuickCode.Demo.Common.Helpers;

namespace QuickCode.Demo.EventListenerService.Models;
public class WorkflowExecutor
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, object> _results = new();
    private readonly Dictionary<string, object> _variables = new();
    private readonly Workflow _workflow;
    private readonly JObject _kafkaMessage;
    public WorkflowExecutor(Workflow workflow, string kafkaMessage, HttpClient httpClient, ILogger logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _kafkaMessage = (JObject)JToken.Parse(kafkaMessage) ;
        _workflow = workflow;
    }
    
    public WorkflowExecutor(string workflowContent, string kafkaMessage, HttpClient httpClient, ILogger logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _kafkaMessage = (JObject)JToken.Parse(kafkaMessage);
        _workflow = WorkflowDeserializer.ParseWorkflow(workflowContent);
    }
    
    public async Task<Dictionary<string, object>> ExecuteWorkflow()
    {
        _logger.LogInformation("Starting workflow: {WorkflowName}", _workflow.Name);
        InitializeVariables(_workflow.Variables);

        foreach (var (stepName, step) in _workflow.Steps)
        {
            step.Url = InitializeBaseUrl(step.Url);
            if (!string.IsNullOrEmpty(step.Condition) && !EvaluateCondition(step.Condition, null))
            {
                _logger.LogInformation("Skipping step {StepName} due to condition", stepName);
                continue;
            }

            if (!string.IsNullOrEmpty(step.Repeat))
            {
                var maxRetries = EvaluateExpression(step.Repeat);
                var currentRetry = 0;
                var success = false;

                while (!success && currentRetry < maxRetries)
                {
                    _variables["currentRetryCount"] = currentRetry;
                    success = await ExecuteStepWithRetry(stepName, step, currentRetry);

                    if (!success)
                    {
                        var delay = CalculateBackoffDelay(currentRetry);
                        await Task.Delay(delay);
                        currentRetry++;
                    }
                }

                if (!success) break;
            }
            else
            {
                if (!await ExecuteStep(stepName, step)) break;
            }
        }

        _logger.LogInformation("Workflow {WorkflowName} completed", _workflow.Name);
        return _results;
    }

    private TimeSpan CalculateBackoffDelay(int retryAttempt)
    {
        var baseDelay = GetVariable<int>("retryBaseDelayMs", 1000);
        var maxDelay = GetVariable<int>("maxRetryDelayMs", 30000);
        
        var exponentialDelay = baseDelay * Math.Pow(2, retryAttempt);
        var withJitter = exponentialDelay * (0.5 + Random.Shared.NextDouble()); 
        
        return TimeSpan.FromMilliseconds(Math.Min(withJitter, maxDelay));
    }

    private T GetVariable<T>(string key, T defaultValue)
    {
        if (_variables.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }

    private async Task<bool> ExecuteStepWithRetry(string stepName, Step step, int currentRetry)
    {
        try
        {
            return await ExecuteStep(stepName, step);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Step {StepName} failed on attempt {Attempt}", stepName, currentRetry + 1);
            return false;
        }
    }

    public static bool IsValidJson(string strInput)
    {
        if (string.IsNullOrWhiteSpace(strInput)) 
        {
            return false;
        }
    
        try
        {
            JsonDocument.Parse(strInput);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
    
    private async Task<bool> ExecuteStep(string stepName, Step step)
    {
        _logger.LogInformation("Executing step: {StepName}", stepName);

        if (step.DependsOn != null && step.DependsOn.Any(dep => !_results.ContainsKey($"{dep}.response")))
        {
            _logger.LogWarning("Dependencies not met for step {StepName}", stepName);
            return false;
        }

        var request = new HttpRequestMessage(new HttpMethod(step.Method), step.Url);
        if (step.Headers != null)
        {
            foreach (var (key, value) in step.Headers)
            {
                var headerValue = ReplaceTokens(value);
                request.Headers.Add(key, headerValue );
            }
        }

        if (step.Body != null)
        {
            var jsonBody = ReplaceTokens(step.Body.ToString());
            request.Content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
        }

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(step.TimeoutSeconds ?? 30));
            var response = await _httpClient.SendAsync(request, cts.Token);
            
            _results[$"{stepName}.statusCode"] = (int)response.StatusCode;
            
            var content = await response.Content.ReadAsStringAsync(cts.Token);

            var result = IsValidJson(content) ? JsonSerializer.Deserialize<JsonElement>(content) : (JsonElement?)null;
            _results[$"{stepName}.response"] = result!;

            if (step.OnSuccess != null)
            {
                foreach (var action in step.OnSuccess)
                {
                    if (EvaluateCondition(action.Condition, result))
                    {
                        await ExecuteStep(action.Action, step.Steps[action.Action]);
                        break;
                    }
                }
            }

            _logger.LogInformation("Step {StepName} completed with status code {StatusCode}", 
                stepName, response.StatusCode);
            return response.IsSuccessStatusCode;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Step {StepName} timed out", stepName);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing step {StepName}", stepName);
            return false;
        }
    }
    
    private void InitializeVariables(Dictionary<string, VariableDefinition> variables)
    {
        foreach (var (key, value) in variables)
        {
            _variables[key] = ParseValue(value.Type, ReplaceTokens(value.Value.ToString()));
        }
    }
    
    private string InitializeBaseUrl(string url)
    {
        return ReplaceTokens(url);
    }
    
    private bool EvaluateCondition(string condition, JsonElement? stepResult)
    {
        if (string.IsNullOrWhiteSpace(condition) || condition == "default")
        {
            return true;
        }

        var andConditions = condition.Split("&&", StringSplitOptions.RemoveEmptyEntries);
        return andConditions.All(andCondition =>
        {
            var orConditions = andCondition.Split("||", StringSplitOptions.RemoveEmptyEntries);
            return orConditions.Any(orCondition => EvaluateSingleCondition(orCondition.Trim(), stepResult));
        });
    }

    private bool EvaluateSingleCondition(string condition, JsonElement? stepResult)
    {
        var match = Regex.Match(condition, @"(\w+(?:\.\w+)*)\s*(==|!=|>|<|>=|<=)\s*(.+)");
        if (!match.Success) return false;

        var leftPart = match.Groups[1].Value.Trim();
        var operatorPart = match.Groups[2].Value.Trim();
        var rightPart = match.Groups[3].Value.Trim();

        var leftValue = GetValue(leftPart, stepResult);
        var rightValue = GetValue(rightPart, stepResult);

        return CompareValues(leftValue, operatorPart, rightValue);
    }

    private object GetValue(string path, JsonElement? stepResult)
    {
        if (path.StartsWith("response.") && stepResult.HasValue)
        {
            var property = path.Substring("response.".Length);
            if (stepResult.Value.TryGetProperty(property, out JsonElement value))
            {
                return value;
            }
        }
        else if (_variables.TryGetValue(path, out var value))
        {
            return value;
        }
        else if (_results.TryGetValue(path, out var resultValue))
        {
            return resultValue;
        }
        return path.Trim('\"');
    }

    private bool CompareValues(object left, string op, object right)
    {
        if (left is JsonElement leftElement && right is JsonElement rightElement)
        {
            return CompareJsonElements(leftElement, op, rightElement);
        }

        if (double.TryParse(left?.ToString(), out double leftDouble) && double.TryParse(right?.ToString(), out double rightDouble))
        {
            return op switch
            {
                "==" => Math.Abs(leftDouble - rightDouble) < 0.000001,
                "!=" => Math.Abs(leftDouble - rightDouble) >= 0.000001,
                ">" => leftDouble > rightDouble,
                "<" => leftDouble < rightDouble,
                ">=" => leftDouble >= rightDouble,
                "<=" => leftDouble <= rightDouble,
                _ => false
            };
        }
        else
        {
            return op switch
            {
                "==" => left?.ToString() == right?.ToString(),
                "!=" => left?.ToString() != right?.ToString(),
                _ => false
            };
        }
    }

    private bool CompareJsonElements(JsonElement left, string op, JsonElement right)
    {
        switch (left.ValueKind)
        {
            case JsonValueKind.Number:
                return CompareValues(left.GetDouble(), op, right.GetDouble());
            case JsonValueKind.String:
                return CompareValues(left.GetString(), op, right.GetString());
            case JsonValueKind.True:
            case JsonValueKind.False:
                return CompareValues(left.GetBoolean(), op, right.GetBoolean());
            default:
                return false;
        }
    }

    private string ReplaceTokens(string input)
    {
        return Regex.Replace(input, @"\{\{(.*?)\}\}", match =>
        {
            var key = match.Groups[1].Value.Trim();

            // Check for specific prefixes and process accordingly
            if (TryExtractKey("QuickCodeClients.", key, out var quickCodeUrl))
            {
                var configApiKey = _configuration.GetValue<string>(key.Replace('.', ':'));
                return configApiKey!.TrimEnd('/');
            }
            
            if (TryExtractKey("QuickCodeApiKeys.", key, out var quickCodeKey))
            {
                var configApiKey = _configuration.GetValue<string>(key.Replace('.', ':'));
                return configApiKey!.TrimEnd('/');
            }

            if (TryExtractKey("variables.", key, out var variableKey))
            {
                return GetValueFromDictionary(_variables, variableKey, match.Value);
            }

            if (TryExtractKey("input.", key, out var inputKey))
            {
                return GetValueFromQueryCollection(_kafkaMessage, inputKey, match.Value);
            }
            
            return GetValueFromDictionary(_results, key, match.Value);
        });
    }
    
    private bool TryExtractKey(string prefix, string fullKey, out string extractedKey)
    {
        if (fullKey.StartsWith(prefix))
        {
            extractedKey = fullKey[prefix.Length..];
            return true;
        }

        extractedKey = string.Empty;
        return false;
    }
    
    private string GetValueFromDictionary(Dictionary<string, object> dictionary, string key, string fallback)
    {
        return dictionary.TryGetValue(key, out var value) ? value?.ToString() ?? fallback : fallback;
    }
    
    private string GetValueFromQueryCollection(JObject value, string key, string fallback)
    {
        return value.GetPathValue(key) ?? fallback;
    }
    
    private int EvaluateExpression(string expression)
    {
        // This is a simple implementation. You might want to use a proper expression evaluator for complex expressions
        return int.Parse(ReplaceTokens(expression));
    }

    private object ParseValue(string type, string value)
    {
        return type switch
        {
            "int" => int.Parse(value),
            "bool" => bool.Parse(value),
            "double" => double.Parse(value),
            _ => value
        };
    }
}