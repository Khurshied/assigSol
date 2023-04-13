using Newtonsoft.Json;

namespace DadJoke.API.Domain
{
    public class Joke
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("setup")]
        public string Setup { get; set; }

        [JsonProperty("punchline")]
        public string Punchline { get; set; }
    }
}
