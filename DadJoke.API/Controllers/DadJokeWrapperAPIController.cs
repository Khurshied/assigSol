using DadJoke.API.Interface;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace DadJoke.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DadJokeWrapperAPIController : ControllerBase
    {
        private readonly IJokeProvider _jokeProvider;        

        public DadJokeWrapperAPIController(IJokeProvider jokeProvider)
        {
            _jokeProvider = jokeProvider;            
        }


        [HttpGet("random/joke")]
        public async Task<IActionResult> GetRandomJoke()
        {

           
            try
            {
                var joke = await _jokeProvider.GetRandomJoke();

                if (joke != null)
                {
                    return Ok(joke);
                }
                else
                {
                    return BadRequest("Failed to fetch joke.");
                }
            }
            catch (Exception ex)
            {                
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }


        [HttpGet("random/jokes")]
        public async Task<IActionResult> GetMultipleRandomJokes([FromQuery] int count)
        {
            
            try
            {
                var jokes = await _jokeProvider.GetMultipleRandomJokes(count);

                if (jokes != null)
                {
                    return Ok(jokes);
                }
                else
                {
                    return BadRequest("Failed to fetch jokes.");
                }
            }
            catch (Exception ex)
            {             
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }



    }
}



