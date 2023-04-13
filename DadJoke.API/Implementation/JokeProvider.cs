using DadJoke.API.Domain;
using DadJoke.API.Interface;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace DadJoke.API.Implementation
{
    public class JokeProvider: IJokeProvider
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy _retryPolicy;


        public JokeProvider(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _retryPolicy = CreateRetryPolicy();
            InitializeHttpClient();
        }

        public async Task<Joke?> GetRandomJoke()
        {
            var singleJokeUrl = _configuration["RapidAPI:EndPoints:SingleJoke"];

            Joke? joke = null;

            await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.GetAsync(singleJokeUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jokeResponseString = await response.Content.ReadAsStringAsync();
                    var jokeApiResponse = JsonConvert.DeserializeObject<JokeApiResponse>(jokeResponseString);

                    if (jokeApiResponse.Success)
                    {
                        joke = jokeApiResponse.Jokes[0];
                    }
                }
            });

            return joke;
        }


        public async Task<IEnumerable<Joke>> GetMultipleRandomJokes(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Count must be greater than zero.", nameof(count));
            }

            var formattedMultipleJokesUrl = _configuration["RapidAPI:EndPoints:MultipleJokes"];
            string multipleJokesUrlWithCount = formattedMultipleJokesUrl.Replace("{count}", count.ToString());

            IEnumerable<Joke> jokes = Enumerable.Empty<Joke>();

            await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.GetAsync(multipleJokesUrlWithCount);

                if (response.IsSuccessStatusCode)
                {
                    var jokeResponseString = await response.Content.ReadAsStringAsync();
                    var jokeApiResponse = JsonConvert.DeserializeObject<JokeApiResponse>(jokeResponseString);

                    if (jokeApiResponse.Success)
                    {
                        jokes = jokeApiResponse.Jokes;
                    }
                }
            });

            return jokes;
        }




        private void InitializeHttpClient()
        {
            var rapidApiKey = _configuration["RapidAPI:Key"];
            var rapidApiHost = _configuration["RapidAPI:Host"];
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", rapidApiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", rapidApiHost);
                      
        }

        private AsyncRetryPolicy CreateRetryPolicy()
        {
            var retryCount = _configuration.GetValue<int>("RapidAPI:RetrySettings:RetryCount");
            var retryDelay = _configuration.GetValue<int>("RapidAPI:RetrySettings:RetryDelay");
            var maxRetryAttempts = _configuration.GetValue<int>("RapidAPI:RetrySettings:MaxRetryAttempts");

            return Policy.Handle<Exception>()
                .WaitAndRetryAsync(retryCount, attempt => TimeSpan.FromSeconds(retryDelay));
        }

    }
}
