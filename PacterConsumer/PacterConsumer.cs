public class PacterConsumer
{
    private readonly HttpClient _httpClient;

    // Constructor that uses IHttpClientFactory for dependency injection
    public PacterConsumer(IHttpClientFactory httpClientFactory)
    {
        // Use the factory to create the named HttpClient instance
        _httpClient = httpClientFactory.CreateClient("Pacter");
        //_httpClient.BaseAddress = new Uri("http://localhost:7071"); // Set the base address of the function app
    }

    // Method to invoke the HelloFunction via HTTP
    public async Task<string> GetGreetingAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/welcome");
            response.EnsureSuccessStatusCode();

            string greeting = await response.Content.ReadAsStringAsync();
            return greeting;
        }
        catch (HttpRequestException e)
        {
            // Handle potential HTTP errors here
            Console.WriteLine($"Request error: {e.Message}");
            return "Error retrieving greeting";
        }
    }
}