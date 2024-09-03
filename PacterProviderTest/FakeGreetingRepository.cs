using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pacter
{
    public class FakeGreetingRepository : IGreetingRepository
    {
        private readonly List<string> _greetings;

        public FakeGreetingRepository()
        {
            _greetings = new List<string>
            {
                "Hello", "Hi", "Hey", "Howdy", "Greetings", "Salutations", "Bonjour",
                "Hola", "Ciao", "Hallo", "Hej", "Olá", "Привет", "你好", "안녕하세요",
                "こんにちは", "Ahoy", "Shalom", "Yassou", "Namaste"
            };
        }

        /// <summary>
        /// Inserts a new greeting into the repository.
        /// </summary>
        /// <param name="greeting">The greeting to add.</param>
        public Task InsertAsync(string greeting)
        {
            _greetings.Add(greeting);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets a random greeting from the repository.
        /// </summary>
        /// <returns>A random greeting string.</returns>
        public string GetRandomGreeting()
        {
            if (!_greetings.Any())
            {
                return string.Empty; // Return an empty string if no greetings are available
            }

            var random = new Random();
            int index = random.Next(_greetings.Count);
            return _greetings[index];
        }

        /// <summary>
        /// Gets all greetings from the repository.
        /// </summary>
        /// <returns>All greeting strings.</returns>
        public IEnumerable<string> GetAllGreetings()
        {
            return _greetings;
        }
    }
}