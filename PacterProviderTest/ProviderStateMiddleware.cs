using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Pacter
{
    public class ProviderStateMiddleware : IFunctionsWorkerMiddleware
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IDictionary<string, Func<IDictionary<string, object>, Task>> providerStates;
        private readonly IGreetingRepository _greetings;
        private readonly ILogger _logger;

        public ProviderStateMiddleware(IGreetingRepository greetings, ILoggerFactory loggerFactory)
        {
            _greetings = greetings;
            _logger = loggerFactory.CreateLogger<ProviderStateMiddleware>();

            providerStates = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                ["a greeting with message {message} exists"] = EnsureGreetingExistsAsync
            };
        }

        private async Task EnsureGreetingExistsAsync(IDictionary<string, object> parameters)
        {
            if (parameters.TryGetValue("message", out var messageObj) && messageObj is JsonElement messageElement)
            {
                string message = messageElement.GetString() ?? string.Empty;
                await _greetings.InsertAsync(message);
            }
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            // Correctly access HttpRequestData from FunctionContext
            var req = await context.GetHttpRequestDataAsync();

            if (req != null && req.Url.AbsolutePath.StartsWith("/provider-states"))
            {
                var response = req.CreateResponse(HttpStatusCode.OK);

                if (req.Method == HttpMethod.Post.Method)
                {
                    string jsonRequestBody;
                    using (var reader = new StreamReader(req.Body))
                    {
                        jsonRequestBody = await reader.ReadToEndAsync();
                    }

                    try
                    {
                        var providerState = JsonSerializer.Deserialize<ProviderState>(jsonRequestBody, Options);

                        if (!string.IsNullOrEmpty(providerState?.State))
                        {
                            if (providerStates.ContainsKey(providerState.State))
                            {
                                await providerStates[providerState.State].Invoke(providerState.Params);
                            }
                            else
                            {
                                _logger.LogWarning("Provider state '{State}' not found", providerState.State);
                                response.StatusCode = HttpStatusCode.BadRequest;
                                await response.WriteStringAsync($"Provider state '{providerState.State}' not found.");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to deserialize JSON provider state body");
                        response.StatusCode = HttpStatusCode.InternalServerError;
                        await response.WriteStringAsync($"Failed to deserialize JSON provider state body: {jsonRequestBody}\n{e}");
                    }

                    // Set the HTTP response in the FunctionContext
                    context.GetInvocationResult().Value = response;

                    return; // Exit early since we handled the request
                }
            }

            // Call the next middleware in the pipeline
            await next(context);
        }
    }
}