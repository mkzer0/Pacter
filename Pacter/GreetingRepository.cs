using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pacter
{
    public class GreetingRepository : IGreetingRepository
    {
        private readonly List<string> _greetings;
        private readonly Random _random;

        public GreetingRepository()
        {
            _greetings = new List<string>
            {
                "Hello", "Hi", "Hey", "Howdy", "Greetings", "Salutations", "Bonjour",
                "Hola", "Ciao", "Hallo", "Hej", "Olá", "Привет", "你好", "안녕하세요",
                "こんにちは", "Ahoy", "Shalom", "Yassou", "Namaste"
            };

            _random = new Random();
        }

        /// <summary>
        /// Inserts a new greeting into the repository asynchronously.
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
            int index = _random.Next(_greetings.Count);
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