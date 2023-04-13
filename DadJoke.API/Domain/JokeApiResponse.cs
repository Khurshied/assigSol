using Newtonsoft.Json;

namespace DadJoke.API.Domain
{
    public class JokeApiResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("body")]
        public Joke[] Jokes { get; set; }
    }
}
