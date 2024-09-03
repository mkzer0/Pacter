using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using Pacter;

public class HelloFunction
{
    private readonly ILogger _logger;
    private readonly IGreetingRepository _greetingRepository;

    public HelloFunction(ILoggerFactory loggerFactory, IGreetingRepository greetingRepository)
    {
        _logger = loggerFactory.CreateLogger<HelloFunction>();
        _greetingRepository = greetingRepository;
    }

    [Function("HelloFunction")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "welcome")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        string greeting = _greetingRepository.GetRandomGreeting();
        response.WriteString(greeting);

        return response;
    }
}