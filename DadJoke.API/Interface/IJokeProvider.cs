using DadJoke.API.Domain;

namespace DadJoke.API.Interface
{
    public interface IJokeProvider
    {
        Task<Joke?> GetRandomJoke();
        Task<IEnumerable<Joke?>> GetMultipleRandomJokes(int count);
    }
}
