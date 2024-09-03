public interface IGreetingRepository
{
    Task InsertAsync(string greeting);
    string GetRandomGreeting();
    IEnumerable<string> GetAllGreetings();
}